import os
from PIL import Image
import sys

def analyze_image(path):
    try:
        with Image.open(path) as img:
            img = img.convert("RGBA")
            width, height = img.size
            bbox = img.getbbox()
            
            if not bbox:
                return "Empty"
            
            # 计算非透明区域的尺寸
            w = bbox[2] - bbox[0]
            h = bbox[3] - bbox[1]
            
            # 计算非透明像素总数 (面积)
            pixels = 0
            for x in range(width):
                for y in range(height):
                    if img.getpixel((x, y))[3] > 0:
                        pixels += 1
            
            return f"Size: {w}x{h}, Area: {pixels}, BBox: {bbox}"
    except Exception as e:
        return f"Error: {e}"

texture_dir = "The Second Seat - Sideria/Textures/Sideria/Narrators/Layered"

print(f"Analyzing textures in {texture_dir}...")

files = [f for f in os.listdir(texture_dir) if f.endswith(".png")]
files.sort()

for f in files:
    path = os.path.join(texture_dir, f)
    info = analyze_image(path)
    print(f"{f}: {info}")
