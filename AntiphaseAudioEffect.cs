using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Audio.Effects;
using YukkuriMovieMaker.Plugin.Effects;

namespace MegriaCore.YMM4.AudioAntiphase
{
    /// <summary>
    /// 逆位相音声エフェクト。
    /// </summary>
    [AudioEffect(EffectName, ["エフェクト"], [], false)]
    public class AntiphaseAudioEffect : AudioEffectBase
    {
        private const string EffectName = "逆位相化";

        /// <summary>
        /// エフェクトの名前
        /// </summary>
        public override string Label
        {
            get
            {
                if (applicableChannel.GetFlag() == AudioChannelFlag.L)
                {
                    return $"{EffectName} - L";
                }
                else if (applicableChannel.GetFlag() == AudioChannelFlag.R)
                {
                    return $"{EffectName} - R";
                }
                else
                {
                    return EffectName;
                }
            }
        }

        // [Display(GroupName = "音声", Name = "適用チャンネル", Description = "エフェクトを適用するチャンネルを指定します。", AutoGenerateField = true)]
        // [EnumComboBox]
        // [DefaultValue(AudioChannelFlag.All)]
        // public AudioChannelFlag ApplicableChannel { get => applicableChannel; set => Set(ref applicableChannel, value, nameof(ApplicableChannel), nameof(Label)); }
        // private AudioChannelFlag applicableChannel = AudioChannelFlag.All;
        // public AudioChannel ApplicableChannel { get => applicableChannel; set => Set(ref applicableChannel, value, nameof(ApplicableChannel), nameof(Label)); }
        public AudioChannel ApplicableChannel => applicableChannel;
        AudioChannel applicableChannel = new(true, true);

        [Display(GroupName = "音声", Name = "L", Description = null)]
        [ToggleSlider]
        public bool ApplicableL { get => applicableChannel._l; set => Set(ref applicableChannel._l, value, nameof(ApplicableL), nameof(Label)); }
        [Display(GroupName = "音声", Name = "R", Description = null)]
        [ToggleSlider]
        public bool ApplicableR { get => applicableChannel._r; set => Set(ref applicableChannel._r, value, nameof(ApplicableR), nameof(Label)); }


        /// <summary>
        /// 音声エフェクトを作成する
        /// </summary>
        /// <param name="duration">音声エフェクトの長さ</param>
        /// <returns>音声エフェクト</returns>
        public override IAudioEffectProcessor CreateAudioEffect(TimeSpan duration)
        {
            return new AntiphaseAudioEffectProcessor(this, duration);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override IEnumerable<string> CreateExoAudioFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// IAnimatableを実装するプロパティを返す
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IAnimatable> GetAnimatables() => [];
    }
    [Flags]
    public enum AudioChannelFlag : byte
    {
        All = 3,
        L = 1,
        R = 2
    }
    public class AudioChannel : Animatable//, IEquatable<AudioChannel>
    {
        [Display(GroupName = "", Name = "L", Description = "")]
        [ToggleSlider]
        public bool L { get => _l; set => Set(ref _l, value); }
        internal bool _l;
        [Display(GroupName = "", Name = "R", Description = "")]
        [ToggleSlider]
        public bool R { get => _r; set => Set(ref _r, value); }
        internal bool _r;

        public AudioChannelFlag GetFlag() => L && R ? AudioChannelFlag.All : L ? AudioChannelFlag.L : R ? AudioChannelFlag.R : 0;

        public AudioChannel(bool L, bool R)
        {
            this.L = L;
            this.R = R;
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];
        /*
        public bool Equals(AudioChannel? other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is null)
                return false;
            if (other.GetType() != this.GetType())
                return false;
            return other.L == L && other.R == R;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as AudioChannel);
        }

        public override int GetHashCode() => HashCode.Combine(L, R);
        */
    }
}
