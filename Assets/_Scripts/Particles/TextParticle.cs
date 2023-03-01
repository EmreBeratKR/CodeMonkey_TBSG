using TMPro;
using UnityEngine;

namespace Particles
{
    public class TextParticle : MonoBehaviour
    {
        [SerializeField] private TMP_Text textField;


        public void SetText(object obj, float fontSize)
        {
            textField.text = obj.ToString();
            textField.fontSize = fontSize;
        }


        private void OnAnimationComplete()
        {
            Destroy(gameObject);
        }
    }
}