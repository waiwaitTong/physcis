using UnityEngine;
using UnityEngine.InputSystem;

public class PoweredConsole : MonoBehaviour
{
    [Header("配置")]
    [SerializeField, Min(0f)] private float initialEnergy = 20f;
    [SerializeField, Min(0f)] private float consumptionPerSecond = 2f;
    [SerializeField, Min(0f)] private float interactionRange = 2f;

    [Header("引用：在 Inspector 中拖入")]
    [SerializeField] private Transform agent;
    [SerializeField] private Light deviceLight;

    [Header("运行状态：运行时观察")]
    [SerializeField] private float currentEnergy;
    [SerializeField] private bool isOn;

    // 后续角色脚本可以读取状态，不能通过这些属性修改状态。
    public float CurrentEnergy => currentEnergy;
    public bool IsOn => isOn;

    private void Start()
    {
        currentEnergy = Mathf.Max(0f, initialEnergy);
        isOn = false;
        RefreshLight();

        if (agent == null || deviceLight == null)
        {
            Debug.LogError("请给 PoweredConsole 配置 Agent 和 Device Light 引用。", this);
            enabled = false;
        }
    }

    private void Update()
    {
        // 暂停时不接受开关操作，也不消耗能量。
        if (Time.timeScale == 0f)
            return;

        // 只在按下 E 的这一帧触发，长按不会反复切换。
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            TryToggle();

        ConsumeEnergy();
        RefreshLight();
    }

    private void TryToggle()
    {
        float distance = Vector3.Distance(agent.position, transform.position);
        if (distance > interactionRange)
        {
            Debug.Log("Agent 距离控制台太远。", this);
            return;
        }

        if (isOn)
        {
            isOn = false;
        }
        else if (currentEnergy > 0f)
        {
            isOn = true;
        }
        else
        {
            Debug.Log("能量不足，无法开启。", this);
        }
    }

    private void ConsumeEnergy()
    {
        if (!isOn)
            return;

        float consumption = Mathf.Max(0f, consumptionPerSecond) * Time.deltaTime;
        currentEnergy = Mathf.Max(0f, currentEnergy - consumption);

        if (currentEnergy <= 0f)
            isOn = false;
    }

    private void RefreshLight()
    {
        if (deviceLight != null)
            deviceLight.enabled = isOn;
    }

    private void OnDisable()
    {
        isOn = false;
        RefreshLight();
    }
}
