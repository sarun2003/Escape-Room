from pydub import AudioSegment, silence
import os


input_file = "Assets/Util/Raw Audio/glass2.mp3"      
output_folder = "Assets/Util/Raw Audio/output/glass2"
min_silence_len = 200            
silence_thresh = -40            
keep_silence = 50                


os.makedirs(output_folder, exist_ok=True)


audio = AudioSegment.from_file(input_file)


chunks = silence.split_on_silence(
    audio,
    min_silence_len=min_silence_len,
    silence_thresh=silence_thresh,
    keep_silence=keep_silence
)


for i, chunk in enumerate(chunks):
    out_file = os.path.join(output_folder, f"glass_{i+3}.mp3")
    chunk.export(out_file, format="mp3")
    print(f"Exported: {out_file}")

print("Done! All footsteps split into individual clips.")