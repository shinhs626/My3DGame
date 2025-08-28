using UnityEngine;
using My3DGame.Manager;
using My3DGame.GameData;

namespace MySample
{
    public class EffectTest : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //EffectA 효과 플레이
            //GameObject effectGo = EffectManager.Instance.EffectOneShot((int)EffectList.EffectB,
            //    this.transform.position);

            SoundManager.Instance.CreateSound(SoundList.Gameplay, this.transform.position);
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SoundManager.Instance.CreateSound(SoundList.EllenAttack01, this.transform.position);
            }
        }
    }
}