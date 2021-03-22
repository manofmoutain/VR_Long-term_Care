using DetectLife;
using UnityEngine;
namespace Global.Pateint
{
    public class PatientTemperature : MonoBehaviour
    {
        [SerializeField] private float[] temperatures;
        [SerializeField] private float temperature;

        public float Temperature => temperature;


        private void Start()
        {
            int randomTemperature = Random.Range(0, temperatures.Length);
            temperature = temperatures[randomTemperature];
        }

        public void ShowTemperature(EarThermometer earThermometer)
        {
            earThermometer.ShowTemperature(Temperature);
        }
    }
}

