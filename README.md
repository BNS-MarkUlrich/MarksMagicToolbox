# MarksMagicToolbox
My Unity Tools and Extensions

## Included in this Package:
### Wave System
Fully configurable and reusable Wave System which can spawn "creatures".

Softlinks:
- TagManager (Used for searching dead bodies in a scene, can easily be replaced by another system)

<details>
  <summary>Inspector Image</summary>
  <img src="https://user-images.githubusercontent.com/71002222/166152218-77b70e0f-b2a2-4f8e-bbc6-0e391754abf5.png" alt="image" width="400"/>
</details>

<br>

### Health System
Efficient Health System that does and has everything you'd expect from a health system.

<details>
  <summary>Inspector Image</summary>
  <img src="https://user-images.githubusercontent.com/71002222/166152539-1a5a9653-cac3-436d-b055-447831c872c2.png" alt="image" width="400"/>
</details>

<br>

### TagManager
An "expansion" on the already existing Tag system from Unity. Currently you can only have one tag on an object, this Tag Manager allows you to add as many per object as you'd like. Also including a Remove method and a HasTag check method.

<details>
  <summary>Inspector Image</summary>
  <img src="https://user-images.githubusercontent.com/71002222/166152754-28f8de43-cf95-4ee9-8a40-8103e2a965d9.png" alt="image" width="400"/>
</details>

#### TagManager Utilities
The TagManager Utilities allow programmers to access the new TagManager even more easily. For example, adding a tag to an object, whether or not it has the tagmanger, can be done straight through a game object, ```gameObject.AddTag("MyTag");```. This line of code will check if the object you want to add a tag to already has a manager, if it does not, it will add one and subsequently also add this tag, if it already has a tagmanager or the tag, it will ignore the command and return.

Other possibilities:
* ```gameObject.RemoveTag("MyTag");``` checks if an object has a tagmanager and if it has the tag. If it does, it will remove the tag, if not, it will simply ignore the command and return.
* ```gameObject.AddTagManager();``` checks if an object has a tagmanager, if not, it will add a tagmanager, if it does already, it will ignore the command and return.
* ```gameObject.HasTagManager();``` checks if an object has a tagmanager.
* ```gameObject.HasTag("MyTag");``` checks if an object has a tag.

<br>

### Transform Utilities
Transform Utilities are method extensions for the transform component native to Unity's Monobehaviour. Some of the possibilities this offers:
* ```gameObject.LookAt(direction)```
* ```gameObject.Hide()```
* ```gameObject.SetPositionX(xPosition)```
* ```gameObject.SetPositionY(yPosition)```
* ```gameObject.SetPositionZ(zPosition)```

<br>

### SceneBodyCleaner
System that clears "dead bodies" from the scene once the amount of dead bodies exceeds the configurable amount.

<details>
  <summary>Inspector Image</summary>
  <img src="https://user-images.githubusercontent.com/71002222/166153755-12a0f1be-12c4-4678-bc7d-96dba17dcf55.png" alt="image" width="400"/>
</details>

###### *This is all to be expanded on later
