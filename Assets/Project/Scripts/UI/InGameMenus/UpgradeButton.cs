using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UpgradeButton : HoverButton
{
    int upgradeBranchIndex;
    int upgradeIndex;
    protected bool isCompleted = false;
    [SerializeField] Upgrade upgrade;
    [SerializeField] GameObject activeNodeImage;
    [SerializeField] GameObject doneText;
    [SerializeField] Image iconImage;
    [SerializeField] ResetableFloatingItem floatingItem;

    UpgradeMenuCanvas upgradeMenuCanvas;


    // Events
    public delegate void UpgradeNodeHoverAction(Vector2 transformPosition);
    public static event UpgradeNodeHoverAction OnUpgradeNodeHover;



    public void Init(bool isEnabled, int upgradeBranchIndex, int upgradeIndex, UpgradeMenuCanvas upgradeMenuCanvas)
    {
        if (isEnabled) EnableButton();
        else DisableButton();

        this.upgradeBranchIndex = upgradeBranchIndex;
        this.upgradeIndex = upgradeIndex;
        this.upgradeMenuCanvas = upgradeMenuCanvas;

        doneText.SetActive(false);
    }



    private void ClickedAnimation()
    {
        transform.DOComplete();
        transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0f), 0.25f, 3);
    }


    public void DisplayUpgrade() // called on hover enter
    {
        upgradeMenuCanvas.DisplayUpgrade(upgrade, isCompleted, upgradeBranchIndex, upgradeIndex);

        if (OnUpgradeNodeHover != null) OnUpgradeNodeHover(GetComponent<RectTransform>().position);
    }

    public void HideDisplay() // called on hover exit
    {
        upgradeMenuCanvas.HideDisplay();
    }

    public virtual void UpgradeSelected() // called on click
    {
        isCompleted = upgradeMenuCanvas.UpgradeSelected(upgradeBranchIndex, upgradeIndex);

        if (isCompleted) DisplayUpgrade();

        ClickedAnimation();
    }

    public virtual void AlwaysProgressUpgradeSelected()
    {
        upgradeMenuCanvas.AlwaysProgressUpgradeSelected(upgradeBranchIndex);
    }




    public void DisableButton()
    {
        GetComponent<Button>().enabled = false;
        GetComponent<Button>().interactable = false;

        activeNodeImage.SetActive(false);

        floatingItem.StopFloating();
    }

    public void EnableButton()
    {
        GetComponent<Button>().enabled = true;
        GetComponent<Button>().interactable = true;

        //activeNodeImage.SetActive(true);

        iconImage.color = new Color(255, 255, 255, 255);

        floatingItem.StartFloating();
    }


    public void SetDone()
    {
        GetComponent<Button>().enabled = false;

        activeNodeImage.SetActive(true);
        doneText.SetActive(true);
        floatingItem.StopFloating();

        isCompleted = true;
    }



    public void GetNameAndIcon(out string upgradeName, out Image upgradeIcon)
    {
        upgradeName = upgrade.upgradeName + " " + upgrade.upgradeDescription;
        upgradeIcon = iconImage;
    }



}
