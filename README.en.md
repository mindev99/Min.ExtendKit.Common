# 🚀 Min.ExtendKit.Common

## 📄 Project Description

A lightweight Windows development toolkit that encapsulates commonly used dialogs, file operations, networking, registry, permissions, security, system information, and Win32 API. Modular design with simple calls, supporting WPF, WinForms, WinUI 3, and more, making Windows development faster and more convenient.

## 📁 Directory Structure

```plaintext
├─ Core
│  ├─ Interfaces    // Shared interfaces between internal modules (few are public)
│  ├─ Models        // Shared internal data models (cross-module reusable)
│  └─ Win32         // Win32 API wrappers, used internally by modules
├─ Dialogs          // Wrappers for file/folder selection dialogs, message boxes, etc.
├─ File             // File operations (copy, move, compress, decompress)
├─ Hardware         // CPU, memory, battery, and other hardware information
├─ Logger           // Logging functionality
├─ Net              // Networking (Ping, port scanning, network adapters info)
├─ Registry         // Registry operations
├─ Security         // Permissions, encryption, and signing
├─ Shell            // Desktop operations, shortcuts, Explorer extensions
└─ SysInfo          // System information (OS, environment variables, directories)
```

## 📝 Notes

* **Core** provides foundational support for modules. External projects generally should not reference Core directly unless using explicitly public models or interfaces.
* Each **module** is self-contained and provides clear APIs for external use.
* External users should call module APIs directly without accessing Core internals.
