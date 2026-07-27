#define UNICODE
#define _UNICODE
#include <windows.h>
#include <shlobj.h>
#include <shellapi.h>
#include <objbase.h>
#include <string>
#include <vector>
#include <atomic>

#if defined(ROBOCOPYDROP_ENGLISH)
static constexpr const wchar_t* kMenuText = L"Copy here via Robocopy";
static constexpr const wchar_t* kRunnerMissingText = L"RobocopyDropRunner.exe missing.";
static constexpr const wchar_t* kRequestErrorText = L"Cannot create the temporary request.";
static constexpr const wchar_t* kStartErrorText = L"Cannot start Robocopy Drop.";
static constexpr const wchar_t* kHelpTextW = L"Copy files and folders to the destination using Robocopy";
static constexpr const char* kHelpTextA = "Copy files and folders to the destination using Robocopy";
#else
static constexpr const wchar_t* kMenuText = L"Copia qui con Robocopy";
static constexpr const wchar_t* kRunnerMissingText = L"RobocopyDropRunner.exe non trovato.";
static constexpr const wchar_t* kRequestErrorText = L"Impossibile creare la richiesta temporanea.";
static constexpr const wchar_t* kStartErrorText = L"Impossibile avviare Robocopy Drop.";
static constexpr const wchar_t* kHelpTextW = L"Copia file e cartelle nella destinazione usando Robocopy";
static constexpr const char* kHelpTextA = "Copia file e cartelle nella destinazione usando Robocopy";
#endif

// {9F3D1E49-A465-4D4D-B570-25EC4D7F4D23}
static const CLSID CLSID_RobocopyDrop =
{ 0x9f3d1e49, 0xa465, 0x4d4d, { 0xb5, 0x70, 0x25, 0xec, 0x4d, 0x7f, 0x4d, 0x23 } };

static HMODULE g_module = nullptr;
static std::atomic<long> g_objectCount{0};
static std::atomic<long> g_serverLocks{0};

static std::wstring QuoteArg(const std::wstring& value) {
    std::wstring result = L"\"";
    unsigned backslashes = 0;
    for (wchar_t ch : value) {
        if (ch == L'\\') {
            ++backslashes;
        } else if (ch == L'\"') {
            result.append(backslashes * 2 + 1, L'\\');
            result.push_back(L'\"');
            backslashes = 0;
        } else {
            result.append(backslashes, L'\\');
            backslashes = 0;
            result.push_back(ch);
        }
    }
    result.append(backslashes * 2, L'\\');
    result.push_back(L'\"');
    return result;
}

static bool GetModuleDirectory(std::wstring& directory) {
    std::vector<wchar_t> buffer(32768);
    DWORD length = GetModuleFileNameW(g_module, buffer.data(), static_cast<DWORD>(buffer.size()));
    if (length == 0 || length >= buffer.size()) return false;
    std::wstring path(buffer.data(), length);
    size_t pos = path.find_last_of(L"\\/");
    if (pos == std::wstring::npos) return false;
    directory = path.substr(0, pos);
    return true;
}

static bool WriteRequestFile(const std::wstring& destination,
                             const std::vector<std::wstring>& sources,
                             std::wstring& requestPath) {
    wchar_t tempPath[32768]{};
    DWORD tempLen = GetTempPathW(static_cast<DWORD>(std::size(tempPath)), tempPath);
    if (tempLen == 0 || tempLen >= std::size(tempPath)) return false;

    wchar_t tempFile[MAX_PATH]{};
    if (GetTempFileNameW(tempPath, L"RCD", 0, tempFile) == 0) return false;
    requestPath = tempFile;

    HANDLE file = CreateFileW(requestPath.c_str(), GENERIC_WRITE, 0, nullptr,
                              CREATE_ALWAYS, FILE_ATTRIBUTE_TEMPORARY, nullptr);
    if (file == INVALID_HANDLE_VALUE) {
        DeleteFileW(requestPath.c_str());
        requestPath.clear();
        return false;
    }

    std::wstring content;
    content.reserve(destination.size() + sources.size() * 128 + 32);
    content.push_back(static_cast<wchar_t>(0xFEFF));
    content.append(destination);
    content.append(L"\r\n");
    for (const auto& source : sources) {
        content.append(source);
        content.append(L"\r\n");
    }

    DWORD bytesWritten = 0;
    const DWORD byteCount = static_cast<DWORD>(content.size() * sizeof(wchar_t));
    BOOL ok = WriteFile(file, content.data(), byteCount, &bytesWritten, nullptr);
    CloseHandle(file);
    if (!ok || bytesWritten != byteCount) {
        DeleteFileW(requestPath.c_str());
        requestPath.clear();
        return false;
    }
    return true;
}

class DragDropHandler final : public IShellExtInit, public IContextMenu {
public:
    DragDropHandler() : refCount_(1) { ++g_objectCount; }
    ~DragDropHandler() { --g_objectCount; }

    IFACEMETHODIMP QueryInterface(REFIID riid, void** ppv) override {
        if (!ppv) return E_POINTER;
        *ppv = nullptr;
        if (IsEqualIID(riid, IID_IUnknown) || IsEqualIID(riid, IID_IShellExtInit)) {
            *ppv = static_cast<IShellExtInit*>(this);
        } else if (IsEqualIID(riid, IID_IContextMenu)) {
            *ppv = static_cast<IContextMenu*>(this);
        } else {
            return E_NOINTERFACE;
        }
        AddRef();
        return S_OK;
    }

    IFACEMETHODIMP_(ULONG) AddRef() override {
        return static_cast<ULONG>(InterlockedIncrement(&refCount_));
    }

    IFACEMETHODIMP_(ULONG) Release() override {
        ULONG count = static_cast<ULONG>(InterlockedDecrement(&refCount_));
        if (count == 0) delete this;
        return count;
    }

    IFACEMETHODIMP Initialize(PCIDLIST_ABSOLUTE pidlFolder, IDataObject* dataObject, HKEY) override {
        destination_.clear();
        sources_.clear();

        if (pidlFolder) {
            wchar_t destination[MAX_PATH]{};
            if (SHGetPathFromIDListW(pidlFolder, destination)) {
                destination_ = destination;
            }
        }

        if (!dataObject) return S_OK;

        FORMATETC format{};
        format.cfFormat = CF_HDROP;
        format.dwAspect = DVASPECT_CONTENT;
        format.lindex = -1;
        format.tymed = TYMED_HGLOBAL;

        STGMEDIUM medium{};
        if (FAILED(dataObject->GetData(&format, &medium))) return S_OK;

        HDROP drop = static_cast<HDROP>(medium.hGlobal);
        if (drop) {
            UINT count = DragQueryFileW(drop, 0xFFFFFFFF, nullptr, 0);
            for (UINT i = 0; i < count; ++i) {
                UINT length = DragQueryFileW(drop, i, nullptr, 0);
                std::vector<wchar_t> path(length + 1);
                if (DragQueryFileW(drop, i, path.data(), static_cast<UINT>(path.size())) > 0) {
                    sources_.emplace_back(path.data());
                }
            }
        }
        ReleaseStgMedium(&medium);
        return S_OK;
    }

    IFACEMETHODIMP QueryContextMenu(HMENU menu, UINT indexMenu, UINT idCmdFirst,
                                    UINT, UINT flags) override {
        if ((flags & CMF_DEFAULTONLY) != 0 || destination_.empty() || sources_.empty()) {
            return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, 0);
        }
        if (!InsertMenuW(menu, indexMenu, MF_BYPOSITION | MF_STRING,
                         idCmdFirst, kMenuText)) {
            return HRESULT_FROM_WIN32(GetLastError());
        }
        return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, 1);
    }

    IFACEMETHODIMP InvokeCommand(CMINVOKECOMMANDINFO* commandInfo) override {
        if (!commandInfo) return E_INVALIDARG;

        bool isOurCommand = false;
        if (HIWORD(commandInfo->lpVerb) == 0) {
            isOurCommand = LOWORD(commandInfo->lpVerb) == 0;
        } else if ((commandInfo->fMask & CMIC_MASK_UNICODE) != 0 &&
                   commandInfo->cbSize >= sizeof(CMINVOKECOMMANDINFOEX)) {
            const auto* infoEx = reinterpret_cast<const CMINVOKECOMMANDINFOEX*>(commandInfo);
            if (infoEx->lpVerbW) {
                isOurCommand = _wcsicmp(infoEx->lpVerbW, L"robocopydrop") == 0;
            }
        } else if (commandInfo->lpVerb) {
            isOurCommand = _stricmp(commandInfo->lpVerb, "robocopydrop") == 0;
        }
        if (!isOurCommand) return E_FAIL;

        std::wstring installDir;
        if (!GetModuleDirectory(installDir)) return E_FAIL;
        std::wstring runner = installDir + L"\\RobocopyDropRunner.exe";
        if (GetFileAttributesW(runner.c_str()) == INVALID_FILE_ATTRIBUTES) {
            MessageBoxW(commandInfo->hwnd, kRunnerMissingText,
                        L"Robocopy Drop", MB_OK | MB_ICONERROR);
            return HRESULT_FROM_WIN32(ERROR_FILE_NOT_FOUND);
        }

        std::wstring request;
        if (!WriteRequestFile(destination_, sources_, request)) {
            MessageBoxW(commandInfo->hwnd, kRequestErrorText,
                        L"Robocopy Drop", MB_OK | MB_ICONERROR);
            return E_FAIL;
        }

        std::wstring commandLine = QuoteArg(runner) + L" " + QuoteArg(request);
        STARTUPINFOW startup{};
        startup.cb = sizeof(startup);
        PROCESS_INFORMATION process{};
        std::vector<wchar_t> mutableCommand(commandLine.begin(), commandLine.end());
        mutableCommand.push_back(L'\0');

        BOOL started = CreateProcessW(runner.c_str(), mutableCommand.data(), nullptr, nullptr,
                                      FALSE, CREATE_NEW_CONSOLE, nullptr, installDir.c_str(),
                                      &startup, &process);
        if (!started) {
            DWORD error = GetLastError();
            DeleteFileW(request.c_str());
            MessageBoxW(commandInfo->hwnd, kStartErrorText,
                        L"Robocopy Drop", MB_OK | MB_ICONERROR);
            return HRESULT_FROM_WIN32(error);
        }
        CloseHandle(process.hThread);
        CloseHandle(process.hProcess);
        return S_OK;
    }

    IFACEMETHODIMP GetCommandString(UINT_PTR idCmd, UINT flags, UINT*, LPSTR name, UINT cchMax) override {
        if (idCmd != 0 || !name || cchMax == 0) return E_INVALIDARG;
        if (flags == GCS_VERBW) {
            wcsncpy_s(reinterpret_cast<LPWSTR>(name), cchMax, L"robocopydrop", _TRUNCATE);
            return S_OK;
        }
        if (flags == GCS_HELPTEXTW) {
            wcsncpy_s(reinterpret_cast<LPWSTR>(name), cchMax,
                      kHelpTextW, _TRUNCATE);
            return S_OK;
        }
        if (flags == GCS_VERBA) {
            strncpy_s(name, cchMax, "robocopydrop", _TRUNCATE);
            return S_OK;
        }
        if (flags == GCS_HELPTEXTA) {
            strncpy_s(name, cchMax,
                      kHelpTextA, _TRUNCATE);
            return S_OK;
        }
        return E_NOTIMPL;
    }

private:
    volatile LONG refCount_;
    std::wstring destination_;
    std::vector<std::wstring> sources_;
};

class ClassFactory final : public IClassFactory {
public:
    ClassFactory() : refCount_(1) { ++g_objectCount; }
    ~ClassFactory() { --g_objectCount; }

    IFACEMETHODIMP QueryInterface(REFIID riid, void** ppv) override {
        if (!ppv) return E_POINTER;
        *ppv = nullptr;
        if (IsEqualIID(riid, IID_IUnknown) || IsEqualIID(riid, IID_IClassFactory)) {
            *ppv = static_cast<IClassFactory*>(this);
            AddRef();
            return S_OK;
        }
        return E_NOINTERFACE;
    }
    IFACEMETHODIMP_(ULONG) AddRef() override {
        return static_cast<ULONG>(InterlockedIncrement(&refCount_));
    }
    IFACEMETHODIMP_(ULONG) Release() override {
        ULONG count = static_cast<ULONG>(InterlockedDecrement(&refCount_));
        if (count == 0) delete this;
        return count;
    }
    IFACEMETHODIMP CreateInstance(IUnknown* outer, REFIID riid, void** ppv) override {
        if (outer) return CLASS_E_NOAGGREGATION;
        auto* object = new (std::nothrow) DragDropHandler();
        if (!object) return E_OUTOFMEMORY;
        HRESULT result = object->QueryInterface(riid, ppv);
        object->Release();
        return result;
    }
    IFACEMETHODIMP LockServer(BOOL lock) override {
        if (lock) ++g_serverLocks; else --g_serverLocks;
        return S_OK;
    }
private:
    volatile LONG refCount_;
};

extern "C" BOOL WINAPI DllMain(HINSTANCE instance, DWORD reason, LPVOID) {
    if (reason == DLL_PROCESS_ATTACH) {
        g_module = instance;
        DisableThreadLibraryCalls(instance);
    }
    return TRUE;
}

extern "C" __declspec(dllexport) HRESULT WINAPI DllGetClassObject(REFCLSID clsid, REFIID riid, void** ppv) {
    if (!IsEqualCLSID(clsid, CLSID_RobocopyDrop)) return CLASS_E_CLASSNOTAVAILABLE;
    auto* factory = new (std::nothrow) ClassFactory();
    if (!factory) return E_OUTOFMEMORY;
    HRESULT result = factory->QueryInterface(riid, ppv);
    factory->Release();
    return result;
}

extern "C" __declspec(dllexport) HRESULT WINAPI DllCanUnloadNow() {
    return (g_objectCount.load() == 0 && g_serverLocks.load() == 0) ? S_OK : S_FALSE;
}
