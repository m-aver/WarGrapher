# Description of the main classes of the application.

<h2 align="center"> 
  DataAccessLayer 
  <sup>[namespace]</sup> 
</h2> 

### ConfigReader <sup>[static class]</sup>
<p> Helper class that contains methods for reading data from the custom configuration sections specified in the application configuration file. For example, paths to the source data files. </p>

---

### CsvFileContext <sup>[class]</sup>
<p> Serves for reading equipment data from the source data files (.csv). Represents the read data in the form of <a href="#equipitem-abstract-class"><code>EquipItem</code></a> objects. </p>

---

### IEquipmentDataContext <sup>[interface]</sup>
<p> Provides access to external data of equipment. For now it contains only one method for retrieving all accessible items. </p>


<h2 align="center"> 
  Models 
  <sup>[namespace]</sup> 
</h2> 

### Model <sup>[class]</sup>
Represents a state of the application data (e.g. which of equipment items have been selected by user). Notifies observers about changes in the application state.

---

### IModel <sup>[interface]</sup>
Represents an interfaces group that intended for interaction of view-models with the application data model.

---

### ModelFactory <sup>[static class]</sup>
Serves for creating and storing the model instance that is common for each part of the application.

---

### PlotDataCalculation <sup>[abstract class]</sup>
Serves as the base class for calculations of various characteristic of the user-selected equipment combinations and provides the point for implementation and integration of a custom calculation to the application.

---

### EquipItem <sup>[abstract class]</sup>
Represents an equipment item of a game character (e.g. a weapon or a particular armor element). Serves as the base class and provides the base functionality for specific types of equipment.


<h2 align="center"> 
  View-Models 
  <sup>[namespace]</sup> 
</h2> 

### ViewModelBase <sup>[abstract class]</sup>
Serves as the base class for view-models.

---

### EquipItemViewModel <sup>[class]</sup>
Serves as the wrapper around an equipment item for representing this item to a view.

---

### WindowViewModel <sup>[abstract class]</sup>
Represents a view-model that intended to be associated with a window as a view. Serves as the base class for the following view-models.

* <div> 
    <h3> MainWindowViewModel <sup>[class]</sup>  </h3>
    <p> Represents the view-model of the main window. Provides commands and properties to control other window view-models. </p>
  </div>
* <div> 
    <h3> ItemSelectionWindowViewModel <sup>[class]</sup>  </h3>
    <p> Represents the view-model of the item selection window. Provides commands and properties for selecting one item from a source collection. </p>
  </div>
* <div> 
    <h3> GraphWindowViewModel <sup>[class]</sup>  </h3>
    <p> Represents the view-model of the graph window. Provides commands and properties for plotting charts by data supplied from an associated calculation class. </p>
  </div>
* <div> 
    <h3> ErrorWindowViewModel <sup>[class]</sup>  </h3>
    <p> Represents the view-model of the error window. Provides commands and properties for recording occurred errors and exposing them to user. </p>
  </div>

---

### ElementViewModel <sup>[abstract class]</sup>
Represents a view-model that is intended to be associated with a particular element of a window (or page) as a view. For now it is an empty class that is introduced for logical grouping of the following view-models.


* <div> 
    <h3> ItemInputViewModel <sup>[class]</sup>  </h3>
    <p> Represents a view-model that provides commands and properties for input an equipment item as selected by user. For now it supports selection by the item name or through opening the item selection window. </p>
  </div>
* <div> 
    <h3> BodyPartSelectionViewModel <sup>[class]</sup>  </h3>
    <p> Represents a view-model that provides commands and properties for selection of a target part of the game character hitbox. </p>
  </div>
* <div> 
    <h3> CalculationSelectionViewModel <sup>[class]</sup>  </h3>
    <p> Represents a view-model that provides commands and properties for exposing available calculation types and creating the corresponded chart windows. The calculation types are retrieved by searching all public classes that derives from the PlotDataCalculation class in the native assemblies of this application or external custom assemblies that located in the application root folder. Therefore, this allows third-party users to extend the application by adding custom calculations. </p>
  </div>

---

### WindowFactory <sup>[abstract class]</sup>
Serves as the base class for creating windows and associated view-models in the specific factory classes. Configures the created components and provides the view-model to a calling code.


<h2 align="center"> 
  Views 
  <sup>[namespace]</sup> 
</h2> 

### WindowViewBase <sup>[abstract class]</sup>
Serves as the base class for view classes that represent an entire window. For now they are the following classes.

* <div> 
    <h3> MainWindowView <sup>[class]</sup>  </h3>
    <p> Represents the main window bound to the <a href="#-mainwindowviewmodel-class--"><code>MainWindowViewModel</code></a>. In this window, the areas for selecting equipment, the target body part and the calculation type is provided by using instances of various element view-models that bound to corresponding UI-elements. </p>
  </div>
* <div> 
    <h3> ItemSelectionWindowView <sup>[class]</sup>  </h3>
    <p> Represents a window that provides a simple visual interface for selection an equipment item from a list of items icons composed by a <code>ListView</code>. This window is bound to the <a href="#-itemselectionwindowviewmodel-class--"><code>ItemSelectionWindowViewModel</code></a>. </p>
  </div>
* <div> 
    <h3> GraphWindowView <sup>[class]</sup>  </h3>
    <p> Represents a window for exposing results of the selected calculation in the form of a 2-D chart by using the <a href="https://oxyplot.github.io/"><code>OxyPlot</code></a> library. This window is bound to the <a href="#-graphwindowviewmodel-class--"><code>GraphWindowViewModel</code></a>. </p>
  </div>
* <div> 
    <h3> ErrorWindowView <sup>[class]</sup>  </h3>
    <p> Represents a window for displaying errors occurred during the work of the application as a table composed by a <code>DataGrid</code>. This window is bound to the <a href="#-errorwindowviewmodel-class--"><code>ErrorWindowViewModel</code></a>. </p>
  </div>


<h2 align="center"> 
  Common 
  <sup>[namespace]</sup> 
</h2> 

### BodyPart <sup>[enum]</sup>
Contains the constants that represent parts of the game character hitbox.

---

### EquipType <sup>[enum]</sup>
Contains the constants that represent specific types of character equipment. Mostly, it is needed to provide a simple way to specify in views which of equipment types will be accessible for selection by a certain UI-element. .This is a flags enumeration thus it allows to specify various combinations of equipment types.

---

### Extensions <sup>[static class]</sup>
Stores various extension methods.

---

### IObserver and IObservable <sup>[interfaces]</sup>
Represent the standard definition of the Observer pattern. For now they is used to update charts when changes occur in the application data model (when user modifies his choice of equipment).
