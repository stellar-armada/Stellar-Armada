namespace VRKeyboard.Utils
{
    public class Shift : Key
    {
        TMPro.TextMeshProUGUI subscript;

        public override void Awake()
        {
            base.Awake();
            subscript = transform.Find("Subscript").GetComponent<TMPro.TextMeshProUGUI>();
        }

        public override void ShiftKey()
        {
            var tmp = key.text;
            key.text = subscript.text;
            subscript.text = tmp;
        }
    }
}