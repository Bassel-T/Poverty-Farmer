using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public GameObject PlantShop, AnimalShop, ToolShop;
    public GameObject plantButton, animalButton, toolButton;
    public GameObject plantArrow, animalArrow, toolArrow;
    public GameObject[] clickableTiles;
    public GameObject balanceText, turnText;
    public GameObject nextYearCanvas;

    public GameObject cornPrefab, wheatPrefab, cottonPrefab, peanutPrefab, chickenPrefab, pigPrefab, cowPrefab, horsePrefab, shovelPrefab, scythePrefab, tractorPrefab;

    public List<Items> items;

    public int itemIndex;
    public int balance;
    public int yearlyMultiplier;
    public int turn;

    private void Start() {
        balance = 50;
        yearlyMultiplier = 1;
        //Need to set up the initial variables.
        PlantShop = GameObject.Find("PlantShop");
        AnimalShop = GameObject.Find("AnimalShop");
        ToolShop = GameObject.Find("ToolShop");
        PlantShop.SetActive(false);
        AnimalShop.SetActive(false);
        ToolShop.SetActive(false);

        plantButton = GameObject.Find("Plant Button");
        animalButton = GameObject.Find("Animal Button");
        toolButton = GameObject.Find("Tool Button");
        plantArrow = GameObject.Find("Plant Arrow");
        animalArrow = GameObject.Find("Animal Arrow");
        toolArrow = GameObject.Find("Tool Arrow");
        nextYearCanvas = GameObject.Find("NextYearCanvas");

        nextYearCanvas.SetActive(false);
        plantArrow.SetActive(false);
        animalArrow.SetActive(false);
        toolArrow.SetActive(false);

        balanceText = GameObject.Find("Balance Text");
        turnText = GameObject.Find("Turn Text");

        clickableTiles = GameObject.FindGameObjectsWithTag("Tiles");
        foreach (GameObject tile in clickableTiles) {
                tile.GetComponent<Tiles>().active = false;
        }
        
        items = new List<Items> {
            new Items("Corn", 0, 8 ,"Plant", cornPrefab, 0),
            new Items("Wheat", 1, 13, "Plant", wheatPrefab, 0),
            new Items("Cotton", 2, 17, "Plant", cottonPrefab, 0),
            new Items("Peanut", 3, 21, "Plant", peanutPrefab, 0),
            new Items("Chicken", 4, 25, "Animal", chickenPrefab, 0),
            new Items("Pig", 5, 50, "Animal", pigPrefab, 0),
            new Items("Cow", 6, 100, "Animal", cowPrefab, 0),
            new Items("Horse", 7, 300, "Animal", horsePrefab, 0),
            new Items("Shovel", 8, 50, "Tool", shovelPrefab, 0.1),
            new Items("Scythe", 9, 100, "Tool", scythePrefab, 0.5),
            new Items("Tractor", 10, 300, "Tool", tractorPrefab, 1)
        };
    }

    // Button to show the plant menu.
    public void PlantMenu() {
        // Close other menus.
        AnimalShop.SetActive(false);
        ToolShop.SetActive(false);
        // Open Planet Menu.
        PlantShop.SetActive(true);
        plantArrow.SetActive(true);
        animalArrow.SetActive(false);
        toolArrow.SetActive(false);
    }


    // Button to show the tool menu.
    public void ToolMenu() {
        // Close other menus.
        AnimalShop.SetActive(false);
        PlantShop.SetActive(false);
        //Open Tool Menu.
        ToolShop.SetActive(true);
        plantArrow.SetActive(false);
        animalArrow.SetActive(false);
        toolArrow.SetActive(true);
    }


    // Button to show the animal menu.
    public void AnimalMenu() {
        // Close other menus.
        PlantShop.SetActive(false);
        ToolShop.SetActive(false);
        // Open Animal Menu.
        AnimalShop.SetActive(true);
        plantArrow.SetActive(false);
        animalArrow.SetActive(true);
        toolArrow.SetActive(false);
    }


    // Button to choose what's being placed.
    public void Choice(int index) {
        // Check if user can afford the item.
        if (balance >= items[index].price * yearlyMultiplier) {
            // Set a variable to a specific item based on the index.
            itemIndex = index;

            // Set up an arrow that will inform of the right tile. ???


            // Make the tiles clickable.
            foreach (GameObject tile in clickableTiles) {
                if (tile.name.StartsWith(items[itemIndex].type)) {
                    tile.GetComponent<Tiles>().active = true;
                } else {
                    tile.GetComponent<Tiles>().active = false;
                }
            }
        }
    }


    // Button to place down the item.
    public void Place(GameObject foo) {
        //Check if the tile is the proper tile for that.
        if (foo.name == items[itemIndex].type.ToString() + "Tile" && (balance >= items[itemIndex].price)) {
            GameObject newItem = (GameObject)Instantiate(items[itemIndex].prefab, foo.transform, instantiateInWorldSpace: false);
            newItem.transform.localPosition = new Vector3(0, 0, 0);
            balance -= items[itemIndex].price * yearlyMultiplier;
            balanceText.GetComponent<Text>().text = "Balance: $" + balance.ToString();
            foo.GetComponent<Tiles>().ownedIndex = itemIndex;
        }
    }


    // Next year button.
    public void NextYear() {
        // Set everything not-interactable.
        foreach (GameObject tile in clickableTiles) {
            tile.GetComponent<Tiles>().active = false;
        }
        plantArrow.SetActive(false);
        animalArrow.SetActive(false);
        toolArrow.SetActive(false);
        // Do crazy math.
        int baseRevenue = 0;
        int money = 0;
        double multiply = 0;

        // Add up the revenue gained from each of the items (though slightly randomized) as well as the multiplier from tools.
        foreach (GameObject tile in clickableTiles) {
            if (tile.GetComponent<Tiles>().ownedIndex >= 0) {
                if (items[tile.GetComponent<Tiles>().ownedIndex].type == "Plant" || items[tile.GetComponent<Tiles>().ownedIndex].type == "Animal") {
                    money += (int)(items[tile.GetComponent<Tiles>().ownedIndex].price + (2 * Mathf.Sin(Random.Range(0, 2 * Mathf.PI))));
                } else if (items[tile.GetComponent<Tiles>().ownedIndex].type == "Tool") {
                    multiply += items[tile.GetComponent<Tiles>().ownedIndex].multiplier;
                }
            }
        }
        // Set the base revenue from what you have.
        baseRevenue = (int)(money * (1 + multiply));

        double familyEfficiency = 0;

        // Read how efficient the family is. (Allowed to be over 100%).
        foreach (Human child in GameObject.Find("Family").transform.GetComponentsInChildren<Human>()) {
            familyEfficiency += child.annualOutput;
        }
        // Gives the family income.
        int income = (int)(baseRevenue * familyEfficiency);

        //Calculate the living cost.

        int livingCost = 0;
        foreach (GameObject tile in clickableTiles) {
            if (tile.GetComponent<Tiles>().ownedIndex >= 0) {
                if (items[tile.GetComponent<Tiles>().ownedIndex].type == "Tool") {
                    livingCost += (int)((items[tile.GetComponent<Tiles>().ownedIndex].price / 5) + (2 * Mathf.Sin(Random.Range(0, 2 * Mathf.PI))));
                }
            }
        }
        livingCost += GameObject.Find("Family").transform.childCount * 7;

        int savings = balance;
        balance += income - livingCost;
        foreach (GameObject tile in clickableTiles) {
            if (tile.GetComponent<Tiles>().ownedIndex >= 0) {
                if (items[tile.GetComponent<Tiles>().ownedIndex].type == "Plant") {
                    Destroy(tile.transform.GetChild(0).gameObject);
                    tile.GetComponent<Tiles>().ownedIndex = -1;
                } else if (items[tile.GetComponent<Tiles>().ownedIndex].type == "Animal") {
                    int value = Random.Range(0, balance);
                    if (value < (balance / 1.5f)) {
                        Destroy(tile.transform.GetChild(0).gameObject);
                        tile.GetComponent<Tiles>().ownedIndex = -1;
                    }
                } else if (items[tile.GetComponent<Tiles>().ownedIndex].type == "Tool") {
                    int value = Random.Range(0, balance);
                    if (value < (balance / 2.5f)) {
                        Destroy(tile.transform.GetChild(0).gameObject);
                        tile.GetComponent<Tiles>().ownedIndex = -1;
                    }
                }
            }
        }

        nextYearCanvas.SetActive(true);
        GameObject.Find("MoneyFromItems").GetComponent<Text>().text = "Item Revenue: $" + money.ToString();
        GameObject.Find("MultiplierFromTools").GetComponent<Text>().text = "Tool Multiplier: x" + (1 + multiply).ToString();
        GameObject.Find("BaseRevenue").GetComponent<Text>().text = "Base Revenue: $" + baseRevenue.ToString();
        GameObject.Find("FamilyEfficiency").GetComponent<Text>().text = "Family Efficiency: " + (Mathf.Round((float)(familyEfficiency * 100))).ToString() + "%";
        GameObject.Find("RealIncome").GetComponent<Text>().text = "Household Income: $" + income.ToString();
        GameObject.Find("LivingCost").GetComponent<Text>().text = "Cost of Living: $" + livingCost.ToString();
        GameObject.Find("Savings").GetComponent<Text>().text = "Past Savings: $" + savings.ToString();
        GameObject.Find("NewBalance").GetComponent<Text>().text = "New Balance: $" + balance.ToString();

        foreach (Human child in GameObject.Find("Family").GetComponentsInChildren<Human>()) {
            if (child.health < 100) {
                Destroy(child.gameObject);
            }
        }

        if (GameObject.Find("Family").transform.childCount == 0) {
            GameObject.Find("Continue!").transform.GetChild(0).GetComponent<Text>().text = "Everyone died! Try again?";
        }

        if (balance <= 0) {
            GameObject.Find("Continue!").transform.GetChild(0).GetComponent<Text>().text = "No more money! Try again?";
        }

    }



        // Button to close the "Next Level" thing.
        public void CloseNext() {
        // Close next menu.
        turn += 1;
        turnText.GetComponent<Text>().text = "Turn: " + turn.ToString();
        balanceText.GetComponent<Text>().text = "Balance: $" + balance.ToString();
        GameObject.Find("Continue!").transform.GetChild(0).GetComponent<Text>().text = "Continue!";
        nextYearCanvas.SetActive(false);
        foreach (Human child in GameObject.Find("Family").GetComponentsInChildren<Human>()) {
            child.newYear();
        }

        if (GameObject.Find("Family").transform.childCount == 0 || balance <= 0) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }

    }

}
