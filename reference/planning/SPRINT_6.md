# 🔭 Sprint 6: Planetary Analysis & Deduction Input (16 Hours)

## Summary

This sprint implements the **Planetary Observatory** (our stand-in for the Gravimeter/Planetary Analyzer). This machine is the player's primary tool for solving the **Portal Equation**. Building it allows the player to measure the hardcoded **Gravimetric Shear** value, which is then used to calculate the exact **Portal Strength Requirement**.

## 🎯 Goal

  * Design, implement, and integrate the **Planetary Observatory** machine.
  * Implement the core Planetary Analysis logic: spending Tech Points to gain the exact numerical value of **Gravimetric Shear**.
  * Update the Scanner UI's Planetary Data section to display this precise, measured value, completing the first half of the deduction loop.

## 💻 Tech Stack Focus

  * **Godot Engine (C\# API):** Machine placement, Tier 2 Tech Point consumption.
  * **C\# / .NET:** Deduction math execution, data storage for measured constants.

-----

## Task Breakdown (16 Hours)

### Task 1: Planetary Observatory Scene and Logic (3 Hours)

| Duration | Steps |
| :--- | :--- |
| **0h - 1h** | **Planetary Observatory Scene Setup**<br>1. Create a new scene `PlanetaryObservatory.tscn` (Area2D root, Sprite2D, CollisionShape2D). This machine should be physically larger than the Field Lab.<br>2. Add a visual element (a dish or telescope) that hints at its measurement function.<br>3. Instance the scene in `World.tscn`. |
| **1h - 2h** | **ObservatoryManager Script**<br>1. Create a C\# script `ObservatoryManager.cs` and attach it.<br>2. Add a `bool IsCalibrated = false;` to track the machine's state.<br>3. Implement a public method `StartCalibration()` that will initiate the measurement. |
| **2h - 3h** | **Player Placement & Tech Point Cost**<br>1. **Cost:** Define a constant in `ObservatoryManager.cs` for the calibration cost (e.g., `const int CalibrationCost = 25;` **Analysis Tech Points**).<br>2. In `PlayerController.cs`, implement a simplified placement/construction logic (Left-Click near the machine).<br>3. When the player "builds" the machine, check if `KnowledgeManager.TechPoints["Analysis"]` $\ge 25$. If true, deduct the points and call `StartCalibration()`. If false, display an error UI message. |

### Task 2: Implementing the Deduction Input (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **3h - 4h** | **Knowledge Manager - Constant Storage**<br>1. In `KnowledgeManager.cs` (or create a new specialized `WorldKnowledge.cs`), define a storage structure for **measured** constants:<br>    - `public Dictionary<string, bool> ConstantsMeasured = new();`<br>    - `public float MeasuredGravity = 0.0f;` (Initial guess is 0). |
| **4h - 5h** | **The Calibration Process with Timer**<br>1. In `ObservatoryManager.cs`, implement the `StartCalibration()` logic:<br>    - Set a state flag: `IsCalibrating = true`<br>    - Start a timer (e.g., 3-5 seconds) with visual/audio feedback for better pacing<br>    - Display a calibration animation (dish spinning, lights pulsing, etc.)<br>    - When timer completes, call `FinishCalibration()`<br>2. In `FinishCalibration()`:<br>    - Set `IsCalibrated = true;`<br>    - Access the true gravity value: `float trueGravity = GameManager.WorldConstants.GravimetricShear;`<br>    - Store the measured value: `KnowledgeManager.Instance.MeasuredGravity = trueGravity;`<br>    - Set the status: `KnowledgeManager.Instance.ConstantsMeasured["Gravity"] = true;`<br>3. **Note:** Adding a calibration timer instead of instant measurement improves game feel and pacing. |
| **5h - 6h** | **Portal Requirement Calculation**<br>1. In the `KnowledgeManager` or `GameManager`, add a method `public float GetCalculatedPortalStrengthRequirement()`.<br>2. IF `KnowledgeManager.ConstantsMeasured["Gravity"]` is true, return: `MeasuredGravity * 2.5f;`<br>3. ELSE, return a high, punishing value (e.g., 99.0f) or simply the original hardcoded target (8.0f) to show the requirement is **unknown**. |
| **6h - 7h** | **Art/Sound: Observatory Assets**<br>1. Create the final 2.5D sprite asset for the **Planetary Observatory** (e.g., a dish antenna or tripod sensor).<br>2. Create sound effects for **"Calibration Start"** (a whirring/charging sound) and **"Measurement Complete"** (a distinct computer report sound). |

### Task 3: Displaying Precise Planetary Data (4 Hours)

| Duration | Steps |
| :--- | :--- |
| **7h - 8h** | **Scanner UI Manager Update (Planetary)**<br>1. In `ScannerUIManager.cs`, modify the `UpdatePlanetaryData(PlanetaryConstants constants)` method (from Sprint 4).<br>2. **New Logic:** Check if Gravity has been measured:<br>    - `bool isMeasured = KnowledgeManager.Instance.ConstantsMeasured.GetValueOrDefault("Gravity");` |
| **8h - 9h** | **Measured vs. Placeholder Display**<br>1. Inside `UpdatePlanetaryData`, implement the display logic for `Label_Gravity`:<br>    - IF `isMeasured` is **true**:<br>        - Display the **measured value** from `KnowledgeManager.MeasuredGravity` (e.g., "Gravity: 3.20 g"). Color the text green/white.<br>    - ELSE IF `isMeasured` is **false**:<br>        - Display the original placeholder (e.g., "Gravity: ??? g"). Color the text yellow/grey. |
| **9h - 10h**| **Updating the Portal Goal UI**<br>1. Modify the `Label` displaying the Portal Goal (from Sprint 3's HUD).<br>2. The text should now call the new method: **"Foundation Strength Required: $>\{KnowledgeManager.GetCalculatedPortalStrengthRequirement():F1\}$"**.<br>3. If the gravity is not measured, the label should still show "Req: $>8.0$" or a similar deduced target (for VS simplicity). Once measured, it shows the exact required value based on the formula: **$3.2 \times 2.5 = 8.0$** (in our hardcoded VS puzzle). |
| **10h - 11h**| **Deduction Confirmation Test**<br>1. Run the game. Check the Portal Goal (e.g., "Req: $>8.0$"). Check the Scanner UI (Gravity: ???).<br>2. Gain enough **Analysis Tech Points** (using the Field Lab repeatedly).<br>3. Build and activate the **Planetary Observatory** (points decrease).<br>4. Check the Scanner UI (Gravity: 3.20 g). Check the Portal Goal (Still shows "Req: $>8.0$"). This confirms the logic and gives the player the required *X* value. |

### Task 4: UI/UX Feedback and Polish (5 Hours)

| Duration | Steps |
| :--- | :--- |
| **11h - 12h**| **Observatory VFX/State**<br>1. Add a VFX to the Observatory. While uncalibrated, it should have a low-power, idle look.<br>2. When `IsCalibrated` is true, the VFX should switch to a high-power, active look (e.g., a dish spinning or lights turning blue). |
| **12h - 13h**| **Construction / Placement UI**<br>1. Implement a simplified **Building Ghost** system for the Observatory: When the player intends to place it, a translucent image of the machine appears under the cursor.<br>2. The ghost should turn **Green** if the player has the points and is in a valid spot, and **Red** if they are missing the points or are placing it on an invalid tile (e.g., water). |
| **13h - 14h**| **Refining Tech Point Display**<br>1. Update the HUD to show the cost for the next Tier 2 tech (Observatory).<br>2. E.g., "Next Tech (Observatory): 25/25 Analysis Points." The cost should glow green when the player can afford it. |
| **14h - 15h**| **Final Review and Testing**<br>1. Verify the full loop: Field Lab (Tier 1) grants points, points unlock Observatory (Tier 2), Observatory measures gravity, gravity updates the numerical requirements on the HUD.<br>2. Ensure the tech point deduction happens correctly and prevents building if the points are insufficient. |
| **15h - 16h**| **Commit Code**<br>1. **Commit Code:** Commit all changes to the VCS with the message: "Sprint 6 Complete: Planetary Observatory Tier 2 Tech, Gravity Measurement, and Deduction Input Implemented." |
