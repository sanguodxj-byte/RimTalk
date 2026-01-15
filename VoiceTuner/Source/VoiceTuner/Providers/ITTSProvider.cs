using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VoiceTuner.Models;

namespace VoiceTuner.Providers
{
    /// <summary>
    /// TTS提供商接口
    /// </summary>
    public interface ITTSProvider
    {
        /// <summary>提供商名称</summary>
        string ProviderName { get; }
        
        /// <summary>提供商描述</summary>
        string ProviderDescription { get; }
        
        /// <summary>提供商类型</summary>
        TTSProviderType ProviderType { get; }
        
        /// <summary>
        /// 合成语音
        /// </summary>
        /// <param name="text">要合成的文本</param>
        /// <param name="config">TTS配置</param>
        /// <returns>音频数据（WAV格式）</returns>
        Task<byte[]?> SynthesizeAsync(string text, TTSProviderConfig config);
        
        /// <summary>
        /// 测试连接
        /// </summary>
        Task<bool> TestConnectionAsync(TTSProviderConfig config);
        
        /// <summary>
        /// 获取可用的语音列表
        /// </summary>
        List<(string Id, string DisplayName)> GetAvailableVoices();
        
        /// <summary>
        /// 绘制配置面板
        /// </summary>
        void DrawConfigPanel(Rect rect, TTSProviderConfig config);

        /// <summary>
        /// 上传音频以克隆音色（可选）
        /// </summary>
        /// <returns>返回音色ID/URI</returns>
        Task<string?> UploadVoiceAsync(string filePath, TTSProviderConfig config);
    }
}