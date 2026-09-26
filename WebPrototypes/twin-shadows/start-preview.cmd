@echo off
cd /d "%~dp0"
echo Open http://127.0.0.1:4317/afterglow/ in your browser.
"C:\Users\JUNGLE\.cache\codex-runtimes\codex-primary-runtime\dependencies\python\python.exe" -m http.server 4317 --bind 127.0.0.1 --directory dist
pause
