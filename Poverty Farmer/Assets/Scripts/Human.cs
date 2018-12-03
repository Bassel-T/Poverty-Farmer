using UnityEngine;
using UnityEngine.UI;

public class Human : MonoBehaviour {

    // Traits
    public bool male; // If true, male. If false, female. For babies.
    public bool fertile;
    public bool pergenat; // Is the lady pregnant? -.-
    public bool userGenerated;

    // Factors of Efficiency
    public double educatedProgress; // A measurement of their education. From 0-10.
    public bool goingToSchool;
    public int age;
    public double health; //HP out of 100.

    // Final Efficiencies
    public double efficiency; //Efficiency, multiple of 1. (Just in case you need it)
    public double annualOutput; // A measurement of how much they are able to output in a given time period.

    // Miscellaneous variables.
    public Game script;
    public GameObject personCanvas;
    public GameObject maleChild, femaleChild;

    private void Start() {
        // Getting the script so I can setup the balance for later.
        script = GameObject.Find("EventSystem").GetComponent<Game>();

        // Set up for other things
        health = 100;
        goingToSchool = false;
        fertile = false;

        if (userGenerated) {
            age = 0;
            educatedProgress = 0;
        }

        annualOutput = resultingOutput(AgeToEfficiency(age), healthToEfficiency(health), educationToEfficiency(educatedProgress));

        // Getting the children so that I can get the buttons working properly.
        personCanvas = this.gameObject.transform.GetChild(0).gameObject;
        personCanvas.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => SellPerson());
        personCanvas.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => SendToSchool());
        personCanvas.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => HealPerson());
        if (!male) {
            personCanvas.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => GiveBirth());
            personCanvas.transform.GetChild(10).GetComponent<Text>().text = "Gender: Female";
        } else {
            personCanvas.transform.GetChild(4).GetChild(0).GetComponent<Text>().text = "He cannot make babies!";
            personCanvas.transform.GetChild(10).GetComponent<Text>().text = "Gender: Male";
        }
        personCanvas.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => CloseMenu());
        personCanvas.transform.GetChild(6).GetComponent<Text>().text = "Age: " + age.ToString();
        personCanvas.transform.GetChild(7).GetComponent<Text>().text = "Health: " + health.ToString();
        personCanvas.transform.GetChild(8).GetComponent<Text>().text = "Education (Max 10): " + educatedProgress.ToString();
        personCanvas.transform.GetChild(9).GetComponent<Text>().text = "Efficiency: " + Mathf.RoundToInt((float)annualOutput).ToString() + "%";
        personCanvas.SetActive(false);

    }

    double AgeToEfficiency(int ageFactor) {
        if (ageFactor <= 7 || ageFactor >= 53) {
            return 0;
        } else {
            return -0.002 * Mathf.Pow((ageFactor - 30), 2) + 1; 
        }
    }

    double healthToEfficiency(double healthFactor) {
        if (healthFactor <= 0 || healthFactor > 100) {
            return 0;
        } else {
            return healthFactor / 100;
        }
    }

    double educationToEfficiency(double educationProgress) {
        if (educationProgress < 2) {
            return 1;
        } else if (educationProgress < 4) {
            return 1.05;
        } else if (educationProgress < 6) {
            return 1.1;
        } else if (educationProgress < 8) {
            return 1.15;
        } else if (educationProgress < 10) {
            return 1.2;
        } else {
            return 1.25;
        }
    }

    double resultingOutput(double ageEff, double healthEff, double eduEff) {
        return ageEff * healthEff * eduEff / 1.25;
    }

    double healthDrop(int ageValue) {
        return 0.02 * Mathf.Pow(ageValue - 30, 2) + 1;
    }

    double SellPrice(double outputRate) {
        return 20 * outputRate;
    }
    
    // This is the new year
    // You are probably looking for this function/
    // It is right here
    // and waiting for you.
    public void newYear() {
        age += 1;
        health -= healthDrop(age);
        if (goingToSchool) {
            educatedProgress += 1;
            goingToSchool = false;
        }

        if (age >= 15) {
            fertile = true;
        }

        if (health <= 0) {
            Destroy(this.gameObject);
        }

        if (pergenat) {
            pergenat = false;
            // Create another child.
            bool newChildIsMale = (Random.Range(0, 1) == 1) ? true : false;
            GameObject newChild = (GameObject)Instantiate(newChildIsMale ? maleChild : femaleChild, new Vector3(0, 0, GameObject.Find("Family").transform.childCount * 1.5f), Quaternion.identity, GameObject.Find("Family").transform);
            newChild.GetComponent<Human>().userGenerated = true;
        }
        personCanvas.transform.GetChild(3).GetComponent<Button>().interactable = true;
        annualOutput = resultingOutput(AgeToEfficiency(age), healthToEfficiency(health), educationToEfficiency(educatedProgress));
    }

    // Buttons from UI

    private void OnMouseDown() {
        personCanvas.SetActive(true);
        personCanvas.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => CloseMenu());
        personCanvas.transform.GetChild(6).GetComponent<Text>().text = "Age: " + age.ToString();
        personCanvas.transform.GetChild(7).GetComponent<Text>().text = "Health: " + health.ToString();
        personCanvas.transform.GetChild(8).GetComponent<Text>().text = "Education (Max 10): " + educatedProgress.ToString();
        personCanvas.transform.GetChild(9).GetComponent<Text>().text = "Efficiency: " + (annualOutput * 100).ToString() + "%";
    }

    public void SellPerson() {
        script.balance += (int)SellPrice(annualOutput);
        script.balanceText.GetComponent<Text>().text = "Balance: " + script.balance.ToString();
        Destroy(this.gameObject);
    }

    public void SendToSchool() {
        annualOutput = 0;
        goingToSchool = true;
        script.balance -= 15;
        script.balanceText.GetComponent<Text>().text = "Balance: " + script.balance.ToString();

    }

    public void HealPerson() {
        if (script.balance >= 20 && health < 100) {
            this.gameObject.GetComponent<Human>().health += 20;
            personCanvas.transform.GetChild(7).GetComponent<Text>().text = "Health: " + health.ToString();
            script.balance -= 20;
            script.balanceText.GetComponent<Text>().text = "Balance: " + script.balance.ToString();
        }

        if (health == 100) {
            personCanvas.transform.GetChild(3).GetComponent<Button>().interactable = false;
        }

    }

    public void GiveBirth() {
        if (fertile) {
            pergenat = true;
        }
    }

    public void CloseMenu() {
        personCanvas.SetActive(false);
    }
    
}
