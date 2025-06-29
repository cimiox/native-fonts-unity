from fontTools.ttLib import TTFont
from PIL import Image, ImageDraw, ImageFont
import os

font_path = '/Users/cimiox/Projects/native_fonts/Assets/Apple Color Emoji.ttc'
output_dir = '/Users/cimiox/Projects/native_fonts/emojies/'

os.makedirs(output_dir, exist_ok=True)

font = TTFont(font_path, fontNumber=0)

cmap = font['cmap']
unicode_map = cmap.getBestCmap()

# Функция для извлечения глифа и создания изображения
def extract_glyph(unicode_codepoint, font_path, output_path, size=160, font_number=0):
    char = chr(unicode_codepoint)
    image = Image.new('RGBA', (size, size), (255, 255, 255, 0))
    draw = ImageDraw.Draw(image)
    
    try:
        font = ImageFont.truetype(font_path, size, index=font_number)
        draw.text((0, 0), char, font=font, fill=(255, 255, 255, 255), embedded_color=True)
        image.save(output_path)
        print(f"Сохранено: {output_path}")
    except OSError as e:
        print(f"Ошибка при рендеринге глифа {char} ({unicode_codepoint}): {e}")

# Извлеките и сохраните emoji
for unicode_codepoint in unicode_map:
    if 0x1F600 <= unicode_codepoint <= 0x1F64F:  # Диапазон кодов для emoji (пример)
        output_path = os.path.join(output_dir, f'{hex(unicode_codepoint)}.png')
        extract_glyph(unicode_codepoint, font_path, output_path, font_number=1)

print('Экспорт завершен.')
