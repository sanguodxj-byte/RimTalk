# TTS音色调节器 Mod 设计方案

## 1. 项目概述

**项目名称**: Voice Tuner / 语音调节器  
**目标**: 创建一个RimWorld mod，提供游戏内TTS音色调节UI，支持多种TTS后端，并能将配置导出为XML格式或直接注入到其他mod（如Sideria）的人格定义中。

## 2. 支持的TTS后端

### 2.1 Azure TTS（微软认知服务）

**API端点**: `https://{region}.tts.speech.microsoft.com/cognitiveservices/v1`

**配置参数**:
| 参数 | 类型 | 范围 | 说明 |
|------|------|------|------|
| `voiceName` | string | - | 语音名称（如 zh-CN-XiaoxiaoNeural） |
| `region` | string | - | 服务区域（如 eastus） |
| `apiKey` | string | - | API密钥 |
| `rate` | float | 0.5-2.0 | 语速倍率 |
| `pitch` | float | 0.5-2.0 | 音调倍率 |
| `style` | string | - | 情感风格（如 cheerful, sad, angry） |
| `styleDegree` | float | 0.01-2.0 | 情感强度 |
| `role` | string | - | 角色扮演（如 Boy, Girl, OlderAdultMale） |

**SSML示例**:
```xml
<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' 
       xmlns:mstts='https://www.w3.org/2001/mstts' xml:lang='zh-CN'>
    <voice name='zh-CN-XiaoxiaoNeural'>
        <mstts:express-as style='cheerful' styledegree='1.5' role='Girl'>
            <prosody rate='+10%' pitch='+5%'>
                你好，这是测试语音。
            </prosody>
        </mstts:express-as>
    </voice>
</speak>
```

### 2.2 Fish Audio

**API端点**: `https://api.fish.audio/v1/tts`

**配置参数**:
| 参数 | 类型 | 范围 | 说明 |
|------|------|------|------|
| `reference_id` | string | - | 参考音色ID |
| `text` | string | - | 要合成的文本 |
| `format` | string | wav/mp3/opus | 输出格式 |
| `latency` | string | normal/balanced | 延迟模式 |
| `streaming` | bool | - | 是否流式输出 |
| `normalize` | bool | - | 是否归一化音量 |
| `chunk_length` | int | 100-300 | 分块长度 |

**请求示例**:
```json
{
  "text": "你好，这是测试语音。",
  "reference_id": "your-voice-id",
  "format": "wav",
  "latency": "normal"
}
```

### 2.3 CosyVoice2（硅基流动 SiliconFlow）

**API端点**: `https://api.siliconflow.cn/v1/audio/speech`

**配置参数**:
| 参数 | 类型 | 范围 | 说明 |
|------|------|------|------|
| `model` | string | - | 模型名称（如 FunAudioLLM/CosyVoice2-0.5B） |
| `input` | string | - | 要合成的文本 |
| `voice` | string | - | 语音ID（如 中文女/中文男/粤语女） |
| `response_format` | string | mp3/wav/opus | 输出格式 |
| `speed` | float | 0.25-4.0 | 语速 |
| `gain` | float | -10 to 10 | 音量增益(dB) |
| `sample_rate` | int | 8000-48000 | 采样率 |

**请求示例**:
```json
{
  "model": "FunAudioLLM/CosyVoice2-0.5B",
  "input": "你好，这是测试语音。",
  "voice": "中文女",
  "response_format": "wav",
  "speed": 1.0
}
```

### 2.4 IndexTTS（本地/自托管）

**API端点**: 本地服务（如 `http://127.0.0.1:8080/tts`）

**配置参数**:
| 参数 | 类型 | 范围 | 说明 |
|------|------|------|------|
| `text` | string | - | 要合成的文本 |
| `speaker` | string | - | 说话人ID |
| `speed` | float | 0.5-2.0 | 语速 |
| `format` | string | wav/mp3 | 输出格式 |

## 3. 数据模型设计

### 3.1 统一TTS配置结构

```csharp
public class TTSProviderConfig
{
    public string ProviderType; // azure, fish_audio, cosyvoice2, index_tts
    public string ApiEndpoint;
    public string ApiKey;
    
    // 通用参数
    public string VoiceId;
    public float Speed = 1.0f;
    public float Pitch = 1.0f;
    public float Volume = 1.0f;
    
    // Azure专用
    public string AzureRegion;
    public string AzureStyle;
    public float AzureStyleDegree = 1.0f;
    public string AzureRole;
    
    // Fish Audio专用
    public string FishReferenceId;
    public string FishLatency = "normal";
    
    // CosyVoice2专用
    public string CosyModel;
    public float CosyGain = 0f;
    public int CosySampleRate = 24000;
}
```

### 3.2 人格TTS配置定义

```csharp
public class PersonaTTSConfig
{
    public string PersonaDefName;      // 关联的人格DefName
    public string PersonaDisplayName;  // 显示名称
    
    public TTSProviderConfig ProviderConfig;
    
    // 预设别名
    public string PresetName;
    
    // 上次测试时间
    public DateTime LastTestTime;
}
```

## 4. 输出XML格式

### 4.1 NarratorPersonaDef兼容格式（用于注入）

```xml
<?xml version="1.0" encoding="utf-8"?>
<Defs>
  <TheSecondSeat.PersonaGeneration.NarratorPersonaDef>
    <defName>Sideria_Default</defName>
    
    <!-- TTS配置（扩展字段） -->
    <ttsVoiceName>zh-CN-XiaoxiaoNeural</ttsVoiceName>
    <ttsVoicePitch>1.0</ttsVoicePitch>
    <ttsVoiceRate>1.0</ttsVoiceRate>
    
    <!-- 扩展：多后端TTS配置 -->
    <ttsProviderType>azure</ttsProviderType>
    <ttsAzureRegion>eastus</ttsAzureRegion>
    <ttsAzureStyle>cheerful</ttsAzureStyle>
    <ttsAzureStyleDegree>1.2</ttsAzureStyleDegree>
    <ttsAzureRole>Girl</ttsAzureRole>
    
    <!-- Fish Audio配置（备选） -->
    <ttsFishReferenceId>voice-id-here</ttsFishReferenceId>
    <ttsFishLatency>normal</ttsFishLatency>
    
    <!-- CosyVoice2配置（备选） -->
    <ttsCosyModel>FunAudioLLM/CosyVoice2-0.5B</ttsCosyModel>
    <ttsCosyVoice>中文女</ttsCosyVoice>
    <ttsCosySpeed>1.0</ttsCosySpeed>
    
  </TheSecondSeat.PersonaGeneration.NarratorPersonaDef>
</Defs>
```

### 4.2 独立配置文件格式

```xml
<?xml version="1.0" encoding="utf-8"?>
<VoiceTunerConfig>
  <Version>1.0</Version>
  <Presets>
    <Preset>
      <Name>Sideria_Cheerful</Name>
      <TargetPersona>Sideria_Default</TargetPersona>
      <Provider>azure</Provider>
      <VoiceName>zh-CN-XiaoxiaoNeural</VoiceName>
      <Pitch>1.0</Pitch>
      <Rate>1.0</Rate>
      <Style>cheerful</Style>
      <StyleDegree>1.2</StyleDegree>
    </Preset>
  </Presets>
</VoiceTunerConfig>
```

## 5. UI设计

### 5.1 主窗口布局

```
┌─────────────────────────────────────────────────────────────┐
│  语音调节器 (Voice Tuner)                            [X]    │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────────┐  ┌──────────────────────────────────────┐  │
│  │ 人格列表    │  │ TTS配置                              │  │
│  ├─────────────┤  ├──────────────────────────────────────┤  │
│  │ ● Sideria   │  │ 提供商: [Azure TTS       ▼]          │  │
│  │   Cassandra │  │                                      │  │
│  │   Phoebe    │  │ ═══════════════════════════════════  │  │
│  │   Randy     │  │                                      │  │
│  │   ...       │  │ 语音: [zh-CN-XiaoxiaoNeural  ▼]      │  │
│  │             │  │                                      │  │
│  │             │  │ 语速: ━━━━━●━━━━━ 1.0x               │  │
│  │             │  │                                      │  │
│  │             │  │ 音调: ━━━━━●━━━━━ 1.0                │  │
│  │             │  │                                      │  │
│  │             │  │ 情感风格: [cheerful      ▼]          │  │
│  │             │  │                                      │  │
│  │             │  │ 情感强度: ━━━━━━●━━━ 1.2              │  │
│  │             │  │                                      │  │
│  │             │  │ 角色: [Girl             ▼]           │  │
│  │             │  │                                      │  │
│  └─────────────┘  └──────────────────────────────────────┘  │
│                                                             │
│  ┌───────────────────────────────────────────────────────┐  │
│  │ 测试文本: [你好，我是Sideria，很高兴认识你。    ]     │  │
│  └───────────────────────────────────────────────────────┘  │
│                                                             │
│  [🔊 播放测试]  [💾 保存配置]  [📤 导出XML]  [📥 注入Mod]   │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

### 5.2 Azure TTS 配置面板

- 区域选择（eastus, westeurope, etc.）
- API密钥输入（密码框）
- 语音选择（下拉列表，按语言分组）
- 语速滑块（0.5x - 2.0x）
- 音调滑块（0.5 - 2.0）
- 情感风格选择（仅Neural语音可用）
- 情感强度滑块（0.01 - 2.0）
- 角色选择（仅部分语音支持）

### 5.3 Fish Audio 配置面板

- API端点（可自定义）
- API密钥
- 参考音色ID
- 延迟模式（normal/balanced）
- 输出格式

### 5.4 CosyVoice2 配置面板

- API端点（硅基流动）
- API密钥
- 模型选择
- 语音选择
- 语速滑块
- 音量增益
- 采样率

## 6. 文件结构

```
VoiceTuner/
├── About/
│   ├── About.xml
│   └── Preview.png
├── Assemblies/
│   └── VoiceTuner.dll
├── Defs/
│   └── (空，配置保存到用户数据目录)
├── Languages/
│   ├── ChineseSimplified/
│   │   └── Keyed/
│   │       └── VoiceTuner_Keys.xml
│   └── English/
│       └── Keyed/
│           └── VoiceTuner_Keys.xml
├── Source/
│   └── VoiceTuner/
│       ├── VoiceTuner.csproj
│       ├── VoiceTunerMod.cs
│       ├── VoiceTunerSettings.cs
│       ├── Models/
│       │   ├── TTSProviderConfig.cs
│       │   ├── PersonaTTSConfig.cs
│       │   └── TTSProviderType.cs
│       ├── Providers/
│       │   ├── ITTSProvider.cs
│       │   ├── AzureTTSProvider.cs
│       │   ├── FishAudioProvider.cs
│       │   ├── CosyVoice2Provider.cs
│       │   └── IndexTTSProvider.cs
│       ├── Services/
│       │   ├── TTSTestService.cs
│       │   └── ConfigExportService.cs
│       ├── UI/
│       │   ├── Window_VoiceTuner.cs
│       │   ├── Panel_ProviderConfig.cs
│       │   ├── Panel_AzureConfig.cs
│       │   ├── Panel_FishAudioConfig.cs
│       │   └── Panel_CosyVoice2Config.cs
│       └── Integration/
│           └── SideriaInjector.cs
└── Textures/
    └── UI/
        └── Icons/
            ├── VoiceTuner.png
            └── Provider_*.png
```

## 7. 核心类设计

### 7.1 ITTSProvider 接口

```csharp
public interface ITTSProvider
{
    string ProviderName { get; }
    string ProviderDescription { get; }
    
    Task<byte[]> SynthesizeAsync(string text, TTSProviderConfig config);
    Task<bool> TestConnectionAsync(TTSProviderConfig config);
    List<string> GetAvailableVoices(TTSProviderConfig config);
    
    void DrawConfigPanel(Rect rect, TTSProviderConfig config);
}
```

### 7.2 ConfigExportService

```csharp
public class ConfigExportService
{
    public static string ExportToXml(PersonaTTSConfig config);
    public static void InjectToMod(PersonaTTSConfig config, string modPath);
    public static PersonaTTSConfig ImportFromXml(string xmlPath);
}
```

## 8. 实现步骤

### Phase 1: 基础框架（1-2天）
1. 创建mod基础结构
2. 实现TTSProviderConfig数据模型
3. 实现基础设置系统

### Phase 2: TTS提供商实现（2-3天）
1. Azure TTS提供商（复用现有代码）
2. Fish Audio提供商
3. CosyVoice2提供商
4. IndexTTS提供商

### Phase 3: 游戏内UI（2-3天）
1. 主窗口框架
2. 人格列表面板
3. 各提供商配置面板
4. 测试播放功能

### Phase 4: 导出与注入（1-2天）
1. XML导出功能
2. Sideria mod注入功能
3. 配置导入功能

### Phase 5: 测试与优化（1天）
1. 功能测试
2. UI优化
3. 文档编写

## 9. 依赖关系

- RimWorld Core
- 0Harmony（已有）
- Newtonsoft.Json（HTTP API调用）
- System.Net.Http（API调用）

## 10. 注意事项

1. **API密钥安全**: 密钥存储在用户本地配置文件中，不随mod分发
2. **网络错误处理**: 所有API调用需要超时和重试机制
3. **音频格式**: 统一转换为WAV格式供RimWorld播放
4. **多线程**: API调用在后台线程执行，避免卡顿
5. **兼容性**: 需要检测The Second Seat mod是否存在