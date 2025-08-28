using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace My3DGame
{
    public class PlayerDataUI : MonoBehaviour
    {
        #region Variables
        public StatsSO statsObejct;

        public Image healthBar;
        public Image manaBar;

        public TextMeshProUGUI levelText;
        public TextMeshProUGUI expText;
        public TextMeshProUGUI goldText;
        #endregion

        #region Unity Event Method
        private void OnEnable()
        {
            statsObejct.OnChangedStats += OnChangedStats;
        }

        private void OnDisable()
        {
            statsObejct.OnChangedStats -= OnChangedStats;
        }

        private void Start()
        {
            UpdatePlayData();
        }
        #endregion

        #region Custom Method
        private void UpdatePlayData()
        {
            healthBar.fillAmount = statsObejct.HealthPercentage;
            manaBar.fillAmount = statsObejct.ManaPrecentage;

            levelText.text = statsObejct.Level.ToString();

            int needForLevelup = statsObejct.GetExpForLevelup(statsObejct.Level);
            expText.text = statsObejct.Exp.ToString() + "/" + needForLevelup.ToString();
            goldText.text = statsObejct.Gold.ToString();
        }

        private void OnChangedStats(StatsSO stats)
        {
            UpdatePlayData();
        }
        #endregion
    }
}
