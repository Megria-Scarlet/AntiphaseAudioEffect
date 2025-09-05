using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Player.Audio.Effects;

namespace MegriaCore.YMM4.AudioAntiphase
{
    public class SamplingDelayEffectProcessor : AudioEffectProcessorBase
    {
        readonly SamplingDelayEffect item;
        readonly TimeSpan duration;

        public SamplingDelayEffectProcessor(SamplingDelayEffect item, TimeSpan duration)
        {
            this.item = item;
            this.duration = duration;
        }

        //出力サンプリングレート。リサンプリング処理をしない場合はInputのHzをそのまま返す。
        /// <summary>
        /// 出力サンプリングレートの値を取得します。
        /// </summary>
        /// <returns>出力サンプリングレート。</returns>
        public override int Hz => Input?.Hz ?? 0;

        public override long Duration //=> (long)(duration.TotalSeconds * Input?.Hz ?? 0) * 2;
        {
            get
            {
                var audio = Input;
                if (audio is null)
                {
                    return 0;
                }
                else
                {
                    long re = AntiphaseAudioEffectProcessor.GetDuration(duration, audio.Hz);

                    re += ((long)item.Delay) * Channel;
                    return re;
                }
            }
        }

        protected override int read(float[] destBuffer, int offset, int count)
        {
            var audio = Input;
            if (audio is null)
            {
                return 0;
            }
            else
            {
                if (emptyLength > 0)
                {
                    if (emptyLength < count)
                    {
                        int i = (int)emptyLength;
                        emptyLength = 0;
                        // Array.Clear(destBuffer, offset, i);
                        destBuffer.AsSpan(offset, i).Clear();
                        audio.Read(destBuffer, offset + i, count - i);
                        return count;
                    }
                    else
                    {
                        emptyLength -= count;
                        // Array.Clear(destBuffer, offset, count);
                        destBuffer.AsSpan(offset, count).Clear();
                        return count;
                    }
                }
                return audio.Read(destBuffer, offset, count);
            }
        }
        private long emptyLength;

        protected override void seek(long position)
        {
            var audio = Input;
            if (audio is not null)
            {
                long delay = (long)item.Delay;

                if (delay >= 0)
                {
                    emptyLength = delay * Channel;
                    if (emptyLength <= position)
                    {
                        audio.Seek(position - emptyLength);
                        emptyLength = 0;
                    }
                    else
                    {
                        audio.Seek(position);
                    }
                }
                else
                {
                    audio.Seek(position + Math.Abs(delay) * Channel);
                }
            }
        }
    }
}
