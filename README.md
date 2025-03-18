# SolviaCopyFilesAtLogon

## 📝 Description
**SolviaCopyFilesAtLogon** is a C# tool designed to automatically copy files when a user logs in. It includes additional features such as font detection and registration, ensuring that copied fonts are properly installed and available in Windows applications. The tool also supports optional email reporting to track file copy operations.

## 🚀 Features
- 📂 **Automatic file copying** at user logon.
- 🔠 **Font detection & registration** (`.ttf`, `.otf`, `.woff`, `.woff2`).
- 📄 **Registry update** to make fonts available without requiring admin rights.
- 📩 **Optional email reporting** for copy operations.
- 📝 **Logging service** for tracking errors and events.

## 📦 Installation
1. Clone the repository:
   ```sh
   git clone https://github.com/itsChris/SolviaCopyFilesAtLogon.git
   ```
2. Open the project in **Visual Studio**.
3. Build the solution.
4. Configure `App.config` with your source and target directories.

## ⚙️ Configuration (`App.config`)
Modify `App.config` to set up paths and optional email reporting:
```xml
<appSettings>
    <add key="SourceDirectory" value="%LOGONSERVER%\Fonts"/>
    <add key="TargetDirectory" value="%LOCALAPPDATA%\SolviaCopiedFiles"/>
    <add key="SendEmail" value="true"/>
    <add key="SmtpServer" value="smtp.example.com"/>
    <add key="SmtpPort" value="587"/>
    <add key="SenderEmail" value="noreply@example.com"/>
    <add key="RecipientEmail" value="admin@example.com"/>
</appSettings>
```

## ▶️ Usage
1. Ensure `App.config` is correctly configured.
2. Run the application, either manually or set it to run at logon.
3. Check logs for status updates.

## 🛠️ How It Works
- The tool **recursively copies files** from the configured source to the target directory.
- If a copied file is a **font**, it is moved to `%LOCALAPPDATA%\Microsoft\Windows\Fonts` and registered.
- Font registration updates **Windows Registry** under:
  ```
  HKEY_CURRENT_USER\Software\Microsoft\Windows NT\CurrentVersion\Fonts
  ```
- A **Windows notification** (`WM_FONTCHANGE`) ensures that the new font is immediately available.
- An **email report** is optionally sent, listing all copied files and their status.

## 📜 License
This project is licensed under the **MIT License**.

## 🤝 Contributing
Pull requests are welcome! Please follow best practices and include proper logging.

---
### ✉️ Contact  
For support or suggestions, contact **Solvia GmbH** at [support@solvia.ch](mailto:support@solvia.ch).
```

