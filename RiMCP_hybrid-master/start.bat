@echo off
title RiMCP Embedding Server
echo 正在启动服务器...

:: 直接使用虚拟环境内的 python.exe 运行，其实不需要显式 activate
"D:\rim mod\RiMCP_hybrid-master\.venv\Scripts\python.exe" "D:\rim mod\RiMCP_hybrid-master\src\RimWorldCodeRag\python\embedding_server.py" --model "D:\rim mod\RiMCP_hybrid-master\src\RimWorldCodeRag\models\e5-base-v2"

:: 如果程序意外退出，暂停窗口让你看清报错
pause