using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Player.Audio.Effects;

namespace MegriaCore.YMM4.AudioAntiphase
{
    public class AntiphaseAudioEffectProcessor : AudioEffectProcessorBase
    {
        readonly AntiphaseAudioEffect item;
        readonly TimeSpan duration;

        //出力サンプリングレート。リサンプリング処理をしない場合はInputのHzをそのまま返す。
        /// <summary>
        /// 出力サンプリングレートの値を取得します。
        /// </summary>
        /// <returns>出力サンプリングレート。</returns>
        public override int Hz => Input is null ? 48000 : Input.Hz;

        public override long Duration //=> (long)(duration.TotalSeconds * Input?.Hz ?? 0) * 2;
        {
            get
            {
                var audio = Input;
                if (audio is null)
                {
                    return GetDuration(duration, 48000);
                }
                else
                {
                    return GetDuration(duration, audio.Hz);
                }
            }
        }

        public AntiphaseAudioEffectProcessor(AntiphaseAudioEffect item, TimeSpan duration)
        {
            this.item = item;
            this.duration = duration;
        }

        protected override int read(float[] destBuffer, int offset, int count)
        {
            var audio = Input;
            if (audio is null)
            {
                destBuffer.AsSpan(offset, count).Clear();
                return count;
            }
            else
            {
                audio.Read(destBuffer, offset, count);
                Span<float> floats = destBuffer.AsSpan(offset, count);

                int i;

                switch (item.ApplicableChannel.GetFlag())
                {
                    case AudioChannelFlag.L:

                        if (floats.Length >= 4)
                        {
                            var vectors = AsVectors(floats);
                            Vector4 mask = Vector4.Create(-1, 1, -1, 1);
                            for (i = 0; i < vectors.Length; i++)
                            {
                                ref var vector = ref vectors[i];
                                vector *= mask;
                            }
                            i = floats.Length & 0x7ffffffC;
                        }
                        else
                        {
                            i = 0;
                        }
                        for (; i < floats.Length; i += 2)
                        {
                            ref float value = ref floats[i];
                            value = -value;
                        }
                        break;
                    case AudioChannelFlag.R:
                        if (floats.Length >= 4)
                        {
                            var vectors = AsVectors(floats);
                            Vector4 mask = Vector4.Create(1, -1, 1, -1);
                            for (i = 0; i < vectors.Length; i++)
                            {
                                ref var vector = ref vectors[i];
                                vector *= mask;
                            }
                            i = floats.Length & 0x7ffffffC;
                        }
                        else
                        {
                            i = 0;
                        }
                        for (i++; i < floats.Length; i += 2)
                        {
                            ref float value = ref floats[i];
                            value = -value;
                        }
                        break;
                    case 0:
                        break;
                    default:

                        if (floats.Length >= 4)
                        {
                            var vectors = AsVectors(floats);
                            for (i = 0; i < vectors.Length; i++)
                            {
                                ref var vector = ref vectors[i];
                                vector = -vector;
                            }
                            i = floats.Length & 0x7ffffffC;
                        }
                        else
                        {
                            i = 0;
                        }

                        for (; i < floats.Length; i++)
                        {
                            ref float value = ref floats[i];
                            value = -value;
                        }
                        break;
                }

                return count;
            }
        }
        private static Span<Vector4> AsVectors(scoped Span<float> floats)
        {
            return MemoryMarshal.CreateSpan(ref Unsafe.As<float, Vector4>(ref MemoryMarshal.GetReference(floats)), floats.Length >> 2);
        }

        protected override void seek(long position)
        {
            Input?.Seek(position);
        }

        internal static long GetDuration(TimeSpan timeSpan, int hertz)
        {
            long i64 = timeSpan.Ticks / TimeSpan.TicksPerSecond;
            long re = i64 * hertz * 2;

            i64 = timeSpan.Ticks - i64 * TimeSpan.TicksPerSecond;
            {
                decimal d = (decimal)i64 / TimeSpan.TicksPerSecond;
                d *= hertz * 2;
                re += (long)d;
            }
            return re;
        }
    }
}
