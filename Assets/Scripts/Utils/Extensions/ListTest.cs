using System.Collections.Generic;
using UnityEngine;

public class ListExtensionsExample : MonoBehaviour
{

    private void Start()
    {
        ExampleUsage();
    }

    public void ExampleUsage()
    {
        List<int> listA = new List<int> { 1, 2, 3, 4, 5 };
        List<int> listB = new List<int> { 2, 4 };

        bool containsAll = listA.Contains(listB); // Check if listA contains all elements in listB
        bool containsAny = listB.ContainsAny(listA); // Check if listA contains any element in listB

        if (containsAll)
            print("listA contains all elements in listB");
            
        if (containsAny) 
            print("listB contains any element in listA");

        listA.AddRange(6, 7, 8); // Add elements to listA

        List<int> listWithout = listA.GetListWithout(listB); // Get a list without elements from listB        

        // add all elements of ListA into single string
        string listAString = string.Join(", ", listA.ToArray()); // 1, 2, 3, 4, 5, 6, 7, 8
        print(listAString);

        // add all elements of ListWithout into single string
        string listWithoutString = string.Join(", ", listWithout.ToArray()); // 1, 3, 5, 6, 7, 8
        print(listWithoutString);
    }
}
