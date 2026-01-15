# ?? AI生成表情差分操作指南

## ?? 目标

为血龙种生成45张表情差分，用于Facial Animation。

---

## ??? 工具选择

### 推荐工具对比

| 工具 | 优点 | 缺点 | 适合场景 |
|------|------|------|---------|
| **Stable Diffusion** | 免费、完全控制、ControlNet | 学习曲线陡 | 有技术背景 |
| **Midjourney** | 高质量、快速 | 付费、控制少 | 追求质量 |
| **NovelAI** | 动漫风格好 | 付费、图层分离难 | 二次元风格 |
| **ComfyUI** | 工作流灵活 | 配置复杂 | 批量生成 |

**本指南推荐**：Stable Diffusion + ControlNet（最灵活）

---

## ?? 准备工作

### 安装Stable Diffusion WebUI

```bash
# Windows
git clone https://github.com/AUTOMATIC1111/stable-diffusion-webui.git
cd stable-diffusion-webui
webui-user.bat

# 首次启动会自动下载模型
```

### 必需的模型

1. **基础模型**
   - `AnythingV5.safetensors` (动漫风格)
   - 或 `CounterfeitV3.safetensors`

2. **ControlNet模型**
   - `control_v11p_sd15_canny`
   - `control_v11p_sd15_openpose`

3. **LoRA模型**（可选）
   - `RimWorld_Style_v1` (如果有)
   - `Chibi_Character_v2`

### 插件安装

```
Settings > Extensions > Available
搜索并安装：
1. ControlNet
2. Segment Anything
3. LayerDiffusion (for透明背景)
```

---

## ?? 生成流程

### 阶段1：生成参考完整角色

#### 步骤1.1：设置基础参数

```
Prompt (正面提示词):
masterpiece, best quality, highly detailed,
anime style, chibi character, game sprite,
isometric view, 45-degree angle,

1girl, female knight, dracovampir,
hooded dark cloak with red trim,
long white silver hair, 
glowing red eyes, dragon horns,
pale skin with red markings,
cold warrior expression, mysterious,

game character design, RimWorld style,
clean lines, cel shading,
transparent background, alpha channel,

Negative Prompt (负面提示词):
lowres, bad anatomy, bad hands, text, error,
missing fingers, extra digit, fewer digits,
cropped, worst quality, low quality,
normal quality, jpeg artifacts, signature, watermark,
username, blurry, artist name,
realistic, 3d, photo, photorealistic,
multiple views, comparison chart
```

#### 步骤1.2：生成设置

```
Width: 512
Height: 512
Sampling method: DPM++ 2M Karras
Sampling steps: 28
CFG Scale: 7
Seed: -1 (或记录满意的seed)
Batch size: 4 (生成多张选最好的)
```

#### 步骤1.3：生成并选择

```
1. 点击Generate生成
2. 选择最符合设定的一张
3. 记录Seed值（用于后续一致性）
4. 保存为reference_full.png
```

---

### 阶段2：生成Base层（无眼嘴）

#### 步骤2.1：使用Inpainting移除眼嘴

```
1. 切换到 img2img > Inpaint
2. 上传reference_full.png
3. 用笔刷涂抹眼睛和嘴巴区域
4. Inpaint设置：
   - Masked content: fill
   - Inpaint area: Only masked
   - Denoising strength: 0.95

Prompt:
same character, same style, same pose,
NO eyes, NO mouth,
smooth facial area where eyes and mouth would be,
transparent in eye and mouth regions

5. Generate → 得到无眼嘴的Base
```

#### 步骤2.2：清理和导出

```
在Photoshop/GIMP中：
1. 打开inpaint结果
2. 使用橡皮擦工具清理眼睛和嘴巴区域
3. 确保这些区域完全透明
4. 导出为Base_north.png (512x512)
```

#### 步骤2.3：生成其他方向

```
重复上述步骤，但修改Prompt：

South (背面):
+ "view from behind, back view"

East (侧面):
+ "side view, profile, facing right"

生成：
- Base_south.png
- Base_east.png
```

---

### 阶段3：生成Eyes层

#### 步骤3.1：设置ControlNet

```
1. 在img2img tab中启用ControlNet
2. 上传reference_full.png
3. Preprocessor: canny
4. Model: control_v11p_sd15_canny
5. Control Weight: 0.8
6. Starting/Ending: 0.0 / 1.0
```

#### 步骤3.2：生成Eyes_Open

```
Prompt:
ONLY eyes, anime eyes, isolated,
glowing red eyes, sharp gaze,
focused warrior eyes,
thin eyebrows slightly furrowed,
transparent background everywhere except eyes,
no face, no mouth, no nose,
just the eye area on transparent canvas

Negative:
full face, mouth, nose, body, hair

Settings:
- Width: 512, Height: 512
- Use ControlNet (使用Base作为参考)
- Denoising: 0.7

Generate
```

#### 步骤3.3：后期处理Eyes

```
在Photoshop中：
1. 打开生成的eyes图
2. 使用Magic Wand选择非眼睛区域
3. Delete删除，留下透明
4. 可能需要手动精修边缘
5. 调整到正确位置（参考坐标）
6. 导出Eyes_Open_north.png
```

#### 步骤3.4：生成所有Eyes变体

复制上述流程，修改Prompt：

**Eyes_Half (半闭)**
```
Prompt修改:
+ "half-closed eyes, sleepy, tired, relaxed"
```

**Eyes_Closed (闭眼)**
```
Prompt修改:
+ "eyes completely closed, peaceful, thin eyelids"
```

**Eyes_Happy (开心)**
```
Prompt修改:
+ "happy eyes, gentle squint, smiling eyes, curved shape"
```

**Eyes_Angry (愤怒)**
```
Prompt修改:
+ "angry eyes, fierce glare, bright red glow, sharp eyebrows"
```

**Eyes_Sad (悲伤)**
```
Prompt修改:
+ "sad eyes, downturned, dimmed glow, melancholic"
```

**Eyes_Pain (痛苦)**
```
Prompt修改:
+ "pained eyes, tightly squinted, grimacing, suffering"
```

每个表情生成North/South/East三个方向。

---

### 阶段4：生成Mouth层

#### 步骤4.1：生成Mouth_Neutral

```
Prompt:
ONLY mouth, anime mouth, isolated,
small closed mouth line, neutral expression,
thin lips, calm, composed,
transparent background everywhere except mouth,
no eyes, no nose, no face,
just the mouth area on transparent canvas

Negative:
eyes, nose, full face, body, hair

Settings同Eyes设置
```

#### 步骤4.2：后期处理Mouth

```
与Eyes相同：
1. 删除非嘴巴区域
2. 留下透明背景
3. 调整位置
4. 导出Mouth_Neutral_north.png
```

#### 步骤4.3：生成所有Mouth变体

**Mouth_Smile**
```
+ "gentle smile, slight upward curve, warm"
```

**Mouth_Frown**
```
+ "frown, downward curve, displeasure"
```

**Mouth_Talk1/2/3**
```
Talk1: "slightly open mouth, oval, speaking"
Talk2: "more open mouth, mid-word"
Talk3: "open mouth, vowel pronunciation"
```

**Mouth_Open**
```
+ "wide open mouth, surprised, shocked"
```

每个表情生成三个方向。

---

## ?? 高级技巧

### 技巧1：使用img2img保持一致性

```
1. 生成第一张满意的Eyes_Open
2. 使用它作为img2img输入
3. 降低Denoising (0.3-0.5)
4. 只修改Prompt描述表情
5. 保持种子不变
→ 风格一致的变体
```

### 技巧2：批量生成脚本

```python
# automatic1111 webui API
import requests

prompts = {
    "Eyes_Open": "sharp focused eyes...",
    "Eyes_Happy": "happy squinted eyes...",
    # ... 其他
}

for name, prompt in prompts.items():
    payload = {
        "prompt": base_prompt + prompt,
        "steps": 28,
        "width": 512,
        "height": 512,
        # ...
    }
    response = requests.post("http://127.0.0.1:7860/sdapi/v1/txt2img", json=payload)
    # 保存结果
```

### 技巧3：使用LayerDiffusion生成透明背景

```
1. 安装LayerDiffusion扩展
2. 在Settings中启用
3. 生成时自动产生透明背景
4. 无需手动抠图
```

### 技巧4：ControlNet Reference保持风格

```
1. 使用你的512x512原图作为Reference
2. ControlNet > Preprocessor: reference_only
3. Model: None
4. Control Weight: 0.5
→ 保持原图风格一致
```

---

## ?? 批量处理工作流

### Photoshop Action自动化

```
1. 打开Action面板
2. 新建Action: "Process_Eyes"
3. 记录步骤：
   - 打开文件
   - Magic Wand选择背景
   - Delete
   - Resize to 128x128
   - Save as PNG-24
4. 批量运行所有文件
```

### Python批量处理脚本

```python
from PIL import Image
import os

def process_layer(input_path, output_path):
    img = Image.open(input_path).convert("RGBA")
    
    # 确保透明背景
    datas = img.getdata()
    newData = []
    for item in datas:
        if item[0] > 250 and item[1] > 250 and item[2] > 250:
            newData.append((255, 255, 255, 0))
        else:
            newData.append(item)
    img.putdata(newData)
    
    # 缩放到128x128
    img = img.resize((128, 128), Image.LANCZOS)
    
    # 保存
    img.save(output_path, "PNG")

# 批量处理
for file in os.listdir("generated/"):
    if file.endswith(".png"):
        process_layer(f"generated/{file}", f"output/{file}")
```

---

## ?? 质量检查清单

### 单文件检查

```
? 尺寸正确 (128x128)
? PNG格式，带Alpha通道
? 透明区域干净（无白边）
? 内容居中对齐
? 与其他文件风格一致
? 文件命名正确
```

### 合成测试

```
在Photoshop中：
1. 新建128x128画布
2. 导入Base
3. 导入Eyes
4. 导入Mouth
5. 检查：
   ? 位置对齐
   ? 无空白
   ? 无重叠
   ? 看起来像完整的脸
```

---

## ?? 时间规划

### 首次生成（学习期）

```
Day 1 (6-8小时):
- 安装和配置工具
- 学习基础操作
- 生成MVP版本（9张）
- 测试显示

Day 2 (4-6小时):
- 生成所有Eyes变体
- 后期处理
- 测试眨眼效果

Day 3 (4-6小时):
- 生成所有Mouth变体
- 后期处理
- 测试说话效果

Day 4 (2-3小时):
- 微调和优化
- 最终质量检查
- 游戏内测试

总计：16-23小时
```

### 熟练后（批量生产）

```
- 每个表情：15-30分钟
- 45张总计：12-22小时
- 后期批处理：2-3小时

总计：14-25小时
```

---

## ?? 故障排除

### 问题1：AI生成的表情不一致

**原因**：种子变化、prompt不稳定

**解决**：
```
1. 固定种子（记录第一张的seed）
2. 使用img2img而非txt2img
3. 降低Denoising strength
4. 使用ControlNet Reference
```

### 问题2：透明背景有白边

**原因**：抠图不干净、PNG导出设置错误

**解决**：
```
1. Photoshop: Layer > Matting > Defringe (1-2px)
2. 手动橡皮擦清理边缘
3. 导出时选择PNG-24，勾选透明度
4. 使用LayerDiffusion扩展直接生成透明背景
```

### 问题3：部件位置对不齐

**原因**：每次生成位置不同

**解决**：
```
1. 在Photoshop中创建参考模板
2. 使用参考线标记位置
3. 手动调整每个图层到正确位置
4. 批量应用offset
```

### 问题4：风格突然变化

**原因**：模型切换、prompt偏移

**解决**：
```
1. 始终使用同一个模型
2. 保持base prompt不变
3. 只修改表情相关的描述词
4. 使用ControlNet保持一致性
```

---

## ?? 参考资源

### Stable Diffusion学习

- [官方Wiki](https://github.com/AUTOMATIC1111/stable-diffusion-webui/wiki)
- [ControlNet教程](https://stable-diffusion-art.com/controlnet/)
- [Prompt工程指南](https://prompthero.com/stable-diffusion-prompt-guide)

### 社区资源

- [CivitAI](https://civitai.com/) - 模型下载
- [Hugging Face](https://huggingface.co/) - 模型和工具
- Reddit r/StableDiffusion - 社区讨论

### 工具

- [Remove.bg](https://www.remove.bg/) - 在线抠图
- [Cleanup.pictures](https://cleanup.pictures/) - AI清理
- [Photopea](https://www.photopea.com/) - 在线Photoshop

---

## ?? 最终检查

完成所有45张后：

```
? 所有文件已生成
? 尺寸全部128x128
? 透明背景正确
? 位置对齐一致
? 风格统一
? 命名规范
? 放入正确文件夹
? 游戏内测试通过
```

---

**准备好开始了吗？**

1. 选择你的AI工具
2. 按照本指南逐步操作
3. 遇到问题查看故障排除部分
4. 完成后进行游戏测试

**预祝生成顺利！** ???

---

需要更详细的某个步骤说明，或遇到具体问题？告诉我，我会提供更详细的帮助！
