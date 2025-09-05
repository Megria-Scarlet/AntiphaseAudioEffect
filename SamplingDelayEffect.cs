using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
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
    /// サンプリング遅延エフェクト。
    /// </summary>
    [AudioEffect(EffectName, ["基本"], [], false)]
    public class SamplingDelayEffect : AudioEffectBase
    {
        private const string EffectName = "サンプリング遅延";
        public override string Label => EffectName;

        /*
         * 数値スライダー
         * アニメーションしない数値を設定するためのコントロールです。
         * [DefaultValue]を設定すると、テキストボックスに空白を入力したときにデフォルト値が設定されます。
         * [Range]を設定すると、数値の範囲を制限できます。
         * 
         * [Display(GroupName = "グループ名", Name = "数値", Description = "項目の説明")]
         * [TextBoxSlider("F1", "%", 0, 100)] 
         * [DefaultValue(0d)]
         * [Range(0,100)]
         */
        [Display(GroupName = "音声", Name = "サンプリング数", Description = "遅延するサンプリング数。")]
        [TextBoxSlider("F0", "Hz", -50000, 50000)]
        [DefaultValue(0d)]
        [Range(-uint.MaxValue, uint.MaxValue)]
        public double Delay { get => delay; set => Set(ref delay, value); }
        protected double delay = 0;

        public SamplingDelayEffect()
        {

        }

        public override IAudioEffectProcessor CreateAudioEffect(TimeSpan duration)
        {
            return new SamplingDelayEffectProcessor(this, duration);
        }

        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public override IEnumerable<string> CreateExoAudioFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            throw new NotSupportedException();
        }

        protected override IEnumerable<IAnimatable> GetAnimatables() => [];
    }
}
