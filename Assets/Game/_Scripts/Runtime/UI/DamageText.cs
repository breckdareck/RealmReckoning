using TMPro;
using UnityEngine;

namespace Game._Scripts.Runtime.UI
{
    public sealed class DamageText : MonoBehaviour
    {
        public TMP_Text dmgtxt;
        public float spacingScale = .5f;
        public float fadeoutTime = .5f;
        public float fadeoutRate = .03f;

        private Color fadeOut;
        private float t;

        private void Awake()
        {
            fadeOut = new Color(0, 0, 0, 0);
            Destroy(gameObject, fadeoutTime + 1f);
        }

        private void Update()
        {
            t += Time.deltaTime;
            transform.position += Vector3.up * Time.deltaTime / 2f;
            if (t >= fadeoutTime) dmgtxt.color = Color.Lerp(dmgtxt.color, fadeOut, fadeoutRate);
        }

        public void SetDamageText(string amount, bool isHeal, bool isCrit)
        {
            /*if (isHeal)
            {
                var dmgtxtColorGradient = dmgtxt.colorGradient;
                dmgtxtColorGradient.topLeft = Color.black;
                dmgtxtColorGradient.bottomLeft = Color.green;
                dmgtxt.colorGradient = dmgtxtColorGradient;
            }

            if (isCrit && !isHeal)
            {
                var dmgtxtColorGradient = dmgtxt.colorGradient;
                dmgtxtColorGradient.topLeft = Color.black;
                dmgtxtColorGradient.bottomLeft = Color.red;
                dmgtxt.colorGradient = dmgtxtColorGradient;
            }*/

            var sb = new System.Text.StringBuilder();
            sb.Append(!isHeal ? "-" : "+");

            for (var i = 0; i < amount.Length; i++)
                if (i % 2 != 0)
                    sb.Append("<voffset=-0.3px>" + amount[i] + "</voffset>");
                else
                    sb.Append(amount[i]);

            dmgtxt.text = sb.ToString();
        }
    }
}