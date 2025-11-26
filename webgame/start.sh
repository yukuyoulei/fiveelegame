#!/bin/bash

# 五行元素冒险游戏启动脚本

echo "正在启动五行元素冒险游戏..."
echo "游戏将在浏览器中打开: http://localhost:8080"
echo "按 Ctrl+C 停止服务器"

# 检查Python是否可用
if command -v python3 &> /dev/null; then
    cd "$(dirname "$0")"
    python3 -m http.server 8080
elif command -v python &> /dev/null; then
    cd "$(dirname "$0")"
    python -m http.server 8080
else
    echo "错误: 未找到Python，请安装Python后重试"
    echo "或者直接在浏览器中打开 index.html 文件"
    exit 1
fi