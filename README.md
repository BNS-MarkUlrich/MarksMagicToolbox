# MarksMagicToolbox
My Unity Tools and Extensions

## Included in this Package:
### TagManager
An "expansion" on the already existing Tag system from Unity. Currently you can only have one tag on an object, this Tag Manager allows you to add as many per object as you'd like. Also including a Remove method and a HasTag check method.

<details>
  <summary>Inspector Image</summary>
  <img src="https://user-images.githubusercontent.com/71002222/166152754-28f8de43-cf95-4ee9-8a40-8103e2a965d9.png" alt="image" width="400"/>
</details>

#### TagManager Utilities
The TagManager Utilities allow programmers to access the new TagManager even more easily. For example, adding a tag to an object, whether or not it has the tagmanger, can be done straight through a game object, ```gameObject.AddTag("MyTag");```. This line of code will check if the object you want to add a tag to already has a manager, if it does not, it will add one and subsequently also add this tag, if it already has a tagmanager or the tag, it will ignore the command and return.

Other possibilities:
* ```TagManagerExtentions.FindAllWithTag("MyTag");``` Returns a list of all Tag Managers that have this tag.
* ```TagManagerExtentions.FindAllWithTags(myList);``` Returns a list of all Tag Managers that has all the tags in myList.
* ```gameObject.RemoveTag("MyTag");``` checks if an object has a tagmanager and if it has the tag. If it does, it will remove the tag, if not, it will simply ignore the command and return.
* ```gameObject.HasTag("MyTag");``` checks if an object has a tag.
* ```gameObject.HasTags("ListOfTags");``` checks if an object has the designated tags.

Softlinks:
- ListExtensions (Used for the `HasTags()` method)

<br>

### Time System
Description to be added soon.

<br>

### Health System
NOTE: The below description is outdated. This is to be updated soon.<br>
Efficient Health System that does and has everything you'd expect from a health system.

<details>
  <summary>Inspector Image</summary>
  <img src="https://user-images.githubusercontent.com/71002222/172047247-57b12925-a8f4-431c-aa2d-ad9edbada0db.png" alt="image" width="400"/>
</details>

<br>

### Wave System
Fully configurable and reusable Wave System which can spawn "creatures".

Softlinks:
- TagManager (Used for searching dead bodies in a scene, can easily be replaced by another system)

<details>
  <summary>Inspector Image</summary>
  <img src="https://user-images.githubusercontent.com/71002222/172047280-795965a5-b95d-4058-9dfe-d205df5dd5b3.png" alt="image" width="400"/>
</details>

<br>

### Transform Utilities
Transform Utilities are method extensions for the transform component native to Unity's Monobehaviour. Some of the possibilities this offers:
* ```gameObject.LookAt(direction);```
* ```gameObject.Hide();```
* ```gameObject.SetPositionX(xPosition);```
* ```gameObject.SetPositionY(yPosition);```
* ```gameObject.SetPositionZ(zPosition);```

<br>

### List Extensions
A new addition to the toolbox, List Extensions will simplify the usage of lists, including but not limited to adding overloads to already existing methods.

Currently my List Extensions includes only one such overload:
* ```myList.Contains(compareList);``` The `Contains()` overload allows you to check wether `myList` includes `compareList`. The lists do not have to be the same size nor do the items have to be in the same index.

<br>

### SceneGarbageCleaner
System that clears "garbage bodies" from the scene once the amount of garbage bodies exceeds the configurable amount.

<details>
  <summary>Inspector Image</summary>
  <img src="https://user-images.githubusercontent.com/71002222/172047303-a8896a5f-d620-47b4-90d4-7de06760b5b3.png" alt="image" width="400"/>
</details>

###### *This is all to be expanded on later
