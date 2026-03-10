# Introduction to 2D Game Development
<table style="margin: 0 auto;">
  <tr>
    <td>
      <img src="./images/game1.png" alt="game1" width="460" height="280">
      <figcaption style="margin-top: 8px; text-align: center;">Moment1</figcaption>
    </td>
    <td style="width: 30px;">
        &nbsp
    </td>
    <td>
      <img src="./images/game2.png" alt="game2" width="460" height="280">
      <figcaption style="margin-top: 8px; text-align: center;">Moment2</figcaption>
    </td>
  </tr>
</table>

---

## COMP561405
### Basic Information
- Course Homepage([Click here](https://html5gameenginegroup.github.io/GTCS-Engine-Student-Projects/2025.3.XJTU/index.html))
- Xi'an Jiaotong University, Computer Experimental Class 2301, 2024-2025 Semester 2
- Repository *Merlin* is the final project,including source code and executable game file for downloading.

### Professor
- 🧑‍🏫 [Kelvin SUNG](https://faculty.washington.edu/ksung/)
- 🧑‍🏫 [Yazhe Tang](https://gr.xjtu.edu.cn/web/yztang)

---

## Merlin
- Be your own magician 🧙
- Explore __a diversity of__ magic elements and 🪄 create your __own__ magic arts
- Travel through __different magical worlds__ and __defeat__ perilous enemies 🦹

### How to Play
- Download the game file `Merlin_v0.1.0_Windows.zip` from the **release**.
- Unzip the game file and run `Merlin.exe` to start your journey! (need Windows Operating System)
- We provide detailed game instructions in the login interface👇.

<img src='./images/login.png' alt='login interface' width='600' height='320'>

### Game's Splendid Moments
[Splendid Moments](https://www.bilibili.com/video/BV1i5QBYBEwg/?vd_source=b0d1dd10fcd3289aa9583d9cb680fb64)


### Key Technologies

Below is a brief overview of the main technical features used in this project. For detailed design explanations, please refer to **[TECHNICAL_DESIGN.md](./TECHNICAL_DESIGN.md)**.

| # | Feature | Brief Description |
|---|---------|-------------------|
| 1 | **Top-down Tilemap** | Dual-layer Unity Tilemap with rich tile sets, supporting smooth camera zoom via Cinemachine. |
| 2 | **Procedural Map & Enemy Generation** | Multi-octave Perlin noise drives four climate-based map themes; enemy types and count scale with exploration depth. |
| 3 | **Save System & Map Persistence** | Binary-serialized world dictionary (`Dictionary<(x,y), SingleScene>`) stores seeds and climate per cell; visiting old maps restores state without regeneration. |
| 4 | **Modular Spell Component & Bullet Architecture** | `SpellEntity` + `SpellLogical` composable design: any bullet entity can be stacked with multiple logical modifiers at runtime via reflection, enabling highly extensible spell combinations. |
| 5 | **Damage & Status System** | Element-specific magic resistance array per entity; 10 status effects (Burn, Frozen, Toxic …) applied by bullet `Hit()` callbacks and ticked each frame. |

### Our Team
💪 Team Merlin, consisting of 4 undergraduates in XJTU, spare no effort to create their own computer magic. 
Team members:
- 👨‍🎓 [Yibo Li](https://github.com/YiboLi-4110)
- 👨‍🎓 [Tianyi Liu](https://github.com/Leotydk671)  
- 👨‍🎓 [Shengqi Zhang](https://github.com/bavarianvilliager) 
- 👨‍🎓 Zihang Yang   
