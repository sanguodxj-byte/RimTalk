import asyncio
import edge_tts
from fastapi import FastAPI
from fastapi.responses import FileResponse
from pydantic import BaseModel
import os
import uuid

# 1. 初始化 FastAPI 应用
app = FastAPI(
    title="Local Edge TTS API",
    description="A local REST API wrapper for edge-tts.",
    version="1.0.0"
)

# 2. 定义请求体的数据模型（修改为接受 float）
class TTSRequest(BaseModel):
    text: str
    voice: str = "zh-CN-XiaoxiaoNeural"
    rate: float = 1.0      # ← 改为 float，默认 1.0
    volume: float = 1.0    # ← 改为 float，默认 1.0

@app.post("/tts")  # ← 端点改为 /tts
async def generate_audio(request: TTSRequest):
    """
    接收文本和语音参数，生成音频文件并返回。
    """
    # 3. 创建唯一的临时文件名
    temp_filename = f"temp_{uuid.uuid4()}.mp3"
    
    try:
        # 4. 将 float 格式转换为 edge-tts 需要的字符串格式
        # rate: 1.0 → "+0%", 1.5 → "+50%", 0.8 → "-20%"
        rate_percent = int((request.rate - 1.0) * 100)
        rate_str = f"+{rate_percent}%" if rate_percent >= 0 else f"{rate_percent}%"
        
        # volume: 1.0 → "+0%", 1.5 → "+50%", 0.5 → "-50%"
        volume_percent = int((request.volume - 1.0) * 100)
        volume_str = f"+{volume_percent}%" if volume_percent >= 0 else f"{volume_percent}%"
        
        # 5. 初始化 edge_tts 通信
        communicate = edge_tts.Communicate(
            request.text, 
            request.voice, 
            rate=rate_str, 
            volume=volume_str
        )
        
        # 6. 生成音频并保存
        await communicate.save(temp_filename)
        
        # 7. 返回音频文件
        return FileResponse(
            temp_filename, 
            media_type='audio/mp3', 
            filename="audio.mp3"
        )
        
    except Exception as e:
        # 8. 异常处理
        return {"error": str(e)}, 500
        
    finally:
        # 9. 延迟清理（确保文件发送完成后再删除）
        # 不在 finally 中立即删除，让 FileResponse 完成发送
        pass

if __name__ == "__main__":
    # 10. 启动 Uvicorn 服务器
    import uvicorn
    uvicorn.run(app, host="127.0.0.1", port=8000)