# Curves Starter — Bring the Axe Home

Open **this folder in Unity 6000.6.0f1. This is the lab.** Do your Cinemachine polish in this lab.

## Scenes

| Scene | Path | Purpose |
| --- | --- | --- |
| Main Game | `Assets/Curves/Scenes/Main Game.unity` | Throw, stick, and recall gameplay |
| Demo | `Assets/Curves/Scenes/[Demo] Bezier.unity` | Scene curves, gizmos, and follower |

## Controls

| Input | When | Effect |
| --- | --- | --- |
| W / S | Any time | Move forward / backward |
| A / D | Any time | Turn left / right |
| Left mouse | Axe **Held** | Start the throw animation |
| Right mouse | Axe **Away** (in flight or stuck) | Recall along the return curve |

Right-click recall works mid-flight. While the axe is **Returning**, input cannot start another throw.

## What is provided

The starter ships working movement and camera, scene references, animation-event release, first-contact stick, recall physics switches, catch with held-pose restoration and state gates, aim-line drawing, follower timing/reset, and EditMode tests.

**Your TODOs:** quadratic and cubic math, scene point/tangent adapters, control-point and curve gizmos, curve line drawing, follower movement/facing, physics launch and designated-target filtering (**4.1–4.2**), bowed return preview, recall timing and motion, and visual spin. Polish and networking are also your assignment work, outside the numbered route. Polish happens in this lab; networking happens in your Resource Collector project.

## How to work the route

Every fill-in is labeled `TODO Slice N.M`. Follow the route card and each TODO's `Next:` line, not source order. Each numbered slice pairs implementation with the hookup needed to see it work. The last fill-in in a slice ends with `</> end of Slice N`; finish that slice's checks before moving on. A TODO marked **(upgrade N.M)** rewrites the code you wrote at TODO N.M; skip it on your first visit to that method.

Each TODO describes a result to achieve. Work out how the math, scene data, and Unity components connect, then use the tests and scene checks to verify your implementation.

## Route card

Paths below are relative to `Assets/Curves/Scripts/`. Complete the instructor-led quadratic walkthrough in the **Demo** scene (slices 1–2), including movement and facing. Then implement and test the cubic independently in the same scene (slice 3), using your completed quadratic work as a reference. Switch to **Main Game** to throw and stick (slice 4) and bring the axe home (slice 5), return to **Demo** for quadratic Bernstein/power basis (slices 6–7), then finish with spin in **Main Game** (slice 8).

| Slice | TODO order and location | Check before continuing |
| --- | --- | --- |
| 1 — Locate the points | **1.1** `Demo/CurveGizmos.Draw` markers → **1.2** `Demo/QuadraticBezierCurve.OnDrawGizmos` hookup → **1.3** helper control polygon | Enable Scene-view Gizmos. Three white wire spheres mark the current world positions; the red polygon connects adjacent points and stays open. Move points outside Play Mode and check that both follow, before implementing curve math. |
| 1 — Draw a quadratic | **1.4** `Bezier/QuadraticBezierMath.SamplePointDeCasteljau` → **1.5** `Demo/QuadraticBezierCurve.SamplePoint` → **1.6** `Demo/CurveGizmos.Draw` sampled curve → **1.7** quadratic `Start` line drawing | Point math test first, then scene-point test. After 1.6, the white gizmo curve reaches both endpoints and responds to moved control points. Restart Play Mode after 1.7; the line matches the gizmos. Follower stays still. |
| 2 — Follow and face | **2.1** `Demo/FollowCurve.Update` position → **2.2** De Casteljau tangent math → **2.3** scene tangent adapter → **2.4** follower facing | After 2.1, watch movement and use `triggerReset` to repeat. After 2.2–2.4, both quadratic tangent tests pass and the follower faces along the curve without warnings. Do not normalize the math derivative. **Discussion check:** explain what the derivative's magnitude measures and why a unit direction alone is not enough; you will need this for the cubic. |
| 3 — Independent cubic | **3.1** `Bezier/CubicBezierMath.SamplePoint` → **3.2** `Demo/CubicBezierCurve.SamplePoint` → **3.3** its `OnDrawGizmos` hookup → **3.4** its `Start` drawing → **3.5** cubic tangent math → **3.6** scene tangent sampling | Implement, connect, and verify this curve on your own. All four cubic tests should pass. Check four markers, an open three-segment control polygon, the visible curve, both endpoints, and changes to its control points. Reuse your completed gizmo helper. Restart Play Mode to refresh the line. The follower remains quadratic. |
| 4 — Throw and stick | **4.1** `Game/ThrownAxe.Launch` physics launch → **4.2** `Game/ThrownAxe.OnCollisionEnter` target filtering | After 4.1, the Launch test passes and a throw flies from the animated release, ignores the player, and sticks on first contact. Then pick your designated target in **Main Game** (the ground, trees, and rocks already have colliders) and decide how to recognize it and what other contacts do. Restructure the supplied first-contact stick so it applies only to that target. Check: a target hit stays stuck until you recall it; a floor or other non-target hit gives your chosen response, visibly different from sticking; recall (still a snap until slice 5) and catch work after both. Animated release and the Returning input rules are unchanged. Keep the Launch test and the four supplied green tests passing. |
| 5 — Bring the axe home | **5.1** `Game/PlayerController.GetReturnControlPoints` bow → **5.2** `DrawReturnPath` sampling → **5.3** `ReturnAxe` timing → **5.4** `ReturnAxe` motion | After 5.2, throw: preview bows from axe to hand while Away; vary `bowAmount`. Preview (5.2) and recall (5.4) must both take their shape from the same `GetReturnControlPoints` definition, not two separate bows. After 5.3, a stuck or mid-flight axe waits `returnDuration` before it snaps home. After 5.4, recall follows that bow with the line hidden: with the player standing still, compare recall against the displayed bow at two or more `bowAmount` values. Demonstrate two complete throw → stick → recall cycles, recall mid-flight, and blocked throw input while Returning. |
| 5 — Optional moving hand | **5.5** adapt `ReturnAxe` for a moving hand | Move/turn during recall: handle/end follow the animated grip while start remains fixed. Then continue to 6.1. |
| 6 — Use Bernstein | **6.1** Bernstein point math → **6.2** switch the scene point adapter → **6.3** Bernstein derivative → **6.4** switch the scene tangent adapter | Run each math test before switching its adapter. Restart Demo and check the same curve, movement, and facing; these are regression checks only, because the old evaluators draw the same curve. **Implementation check:** read both live adapters and confirm each actually calls your Bernstein version. Keep Bernstein in both adapters. |
| 7 — Prepare once, sample many | **7.1** power coefficients → **7.2** power point math → **7.3** upgrade scene line sampling → **7.4** power derivative | Coefficients `(0,0,0), (4,6,0), (0,-6,0)` for points `(0,0,0), (2,3,0), (4,0,0)`. Unchanged control points should not require repeated coefficient preparation. Line matches Bernstein gizmos; all 17 tests pass after 7.4. Matching output is a regression check, not proof of the upgrade. **Implementation check:** read your `Start` line code and confirm coefficients are prepared once, outside the sample loop, and every sample in that batch uses power evaluation. Be ready to explain where repeated preparation is avoided. |
| 8 — Spin and connect | **8.1** `Game/ThrownAxe` visual-child setup → **8.2** visual spin behavior → **8.3** launch/contact/recall/catch hooks in `ThrownAxe` and `PlayerController` | First verify that separating the visual preserves held appearance and throw/catch. Then verify throw spin stops when the axe sticks in your target. Connect recall and catch: both legs spin, stuck/held do not, and two catches restore the original held appearance. |

For basic **5.4**, the return path stays fixed for the duration of recall. Test with the player still: moving during this version can cause a small correction to the live hand at catch. Optional **5.5** adapts the curve to the moving grip while preserving the original start.

Bernstein and power basis both have analytic tangents. After slice 7, live quadratic scene queries use Bernstein and read current world positions; the line uses coefficients prepared for one fixed batch in `Start`. Restart Play Mode after moving control points to refresh that line. Gizmos and follower sampling do not depend on `Start`. Axe preview/recall continue using De Casteljau directly; the assignment accepts any of your evaluators for recall. No runtime selector is needed.

The cubic exercise is required and graded: its four supplied tests count toward row 2. It covers De Casteljau points, tangents, scene sampling, gizmo hookup, and line drawing; it does not require a cubic follower or a cubic axe return.

## Graded work

Finishing every TODO in this project is **not** the whole assignment. The course assignment page is the grading authority (required work, polish, networking, video, and rubric evidence).

| Row | Topic | Pts | Supplied / your work |
| --- | --- | --: | --- |
| 1 | Showcase | 10 | There is no starter code for this row: make the required video and submit it with the repository link. |
| 2 | Three quadratic implementations (Math) | 10 | Fill math TODOs **1.4, 2.2, 6.1, 6.3, 7.1, 7.2, 7.4** and their scene hookups, and pass the four cubic tests from slice 3; show code and test results. Explain or demonstrate coefficient preparation once and reuse while control points stay unchanged. Tests check values, not caching discipline or independent derivation. |
| 3 | Curve-driven recall | 25 | Fill **5.1–5.4** with an obvious bow: recall timing (5.3) advances `t` from 0 to 1 and motion (5.4) samples the curve at it. The assignment accepts any of your evaluators; this route uses your quadratic De Casteljau evaluator directly—no scene-curve Inspector binding. |
| 4 | Axe spin | 10 | Required student design at **8.1–8.3**, on both throw and recall; there is no empty spin method to fill. |
| 5 | Throw and stick | 10 | First-hit sticking is supplied; the physics launch is yours (**4.1**). Once launched, the axe sticks to the first collider it touches, including the floor. You must choose a designated target and implement target filtering in ThrownAxe.OnCollisionEnter; a floor hit alone does not satisfy this row. You also choose what happens on non-target collisions; those must not count as target success. Route: TODO 4.1–4.2. |
| 6 | Animated release | 5 | Supplied: preserve **ThrowAction → LaunchAxe → ThrownAxe.Launch**; do not replace it with a new launch at mouse-down. |
| 7 | Recall and repeat | 10 | Catch and state gates are supplied; recall timing and motion are **5.3–5.4**. Preserve cleanup and demonstrate at least two full throw → stick → recall cycles without restarting Play Mode. |
| 8 | Polish | 10 | There is no starter code for this row: implement four qualifying assignment improvements at 2.5 points each. Existing features do not automatically earn improvement credit. Cinemachine work belongs here. |
| 9 | Networking | 10 | There is no starter code for this row: bring your throw and recall, including the curved recall and spin, into your Resource Collector project so host and client each throw and recall their own axe. Required, not optional. |
| — | Extra credit: Breakable objects | +5 | Optional. Tree health / auto-return is optional polish, not required core. |

## What you will see before you start

### Main Game

The throw animation releases the axe, which detaches and hangs in the air in front of the hand until you add the physics launch (**4.1**). Right-click from **Away** freezes the axe at its current world position, disables collision, and on the next frame `AttachToHand` snaps it to the live hand grip and returns to **Held**; recall timing is **5.3**. Because the recall coroutine starts with no position assignment, the axe does **not** teleport to the origin.

The held aim line is supplied. The return preview is initially empty; it appears after **5.1–5.2** and remains hidden during recall. Implementing math alone does not draw it or move the axe.

On the **Main Game** scene, `returnDuration` is overridden to **0.5 s**. The **Player** prefab default is **1 s** if you test from the prefab alone.

No exceptions or recurring error logs are expected. Both scenes should open with zero missing scripts.

### Demo scene (`[Demo] Bezier`)

Initially there are no custom curve gizmos: markers, control polygons, sampled curves, and scene hookups are your work. Both curve evaluators return zero, and both Play Mode lines start empty. Enable **Gizmos** in the Scene view while working through the route.

Quadratic markers appear after **1.1–1.2**, its control polygon after **1.3**, and its sampled gizmo curve after **1.4–1.6**. Its Play Mode line follows at **1.7**. Cubic gizmos appear after **3.1–3.3**, reusing your completed helper; its line follows at **3.4**. Move the control points in Edit Mode at each gizmo checkpoint to check the geometry. The follower stays in its authored pose until **2.1**.

The unfinished follower has no position or forward assignment. No repeated zero-direction warning is expected. Add facing only after the tangent math and adapter work (**2.2–2.3**). Do not disable the follower to hide a problem.

Scene lines are sampled in `Start`: restart Play Mode after filling or changing their code. Check the Console at each slice; compile errors, missing references, and recurring warnings are not expected.

## Test Runner

Open **Window → General → Test Runner**, choose **EditMode**, and run all tests in `Assets/Tests/EditMode/`.

**Starting result:** **17** test cases — **13 fail**, **4 pass** — with no compile, discovery, or setup errors.

### Intentional failures (go green when you complete the TODOs)

| Test | Goes green after |
| --- | --- |
| `CurveTests.DeCasteljauQuadratic_SamplesPointFromEquivalentQuadraticFormula` | 1.4 |
| `CurveTests.SceneCurve_SamplesCurrentWorldPointsWithoutCallbacks(False,False)` | 1.4 + 1.5; stays green after 6.1 + 6.2 |
| `CurveTests.DeCasteljauQuadratic_SamplesTangentFromFinalInterpolationSegment` | 2.2 |
| `CurveTests.SceneCurve_SamplesCurrentWorldPointsWithoutCallbacks(False,True)` | 2.2 + 2.3; stays green after 6.3 + 6.4 |
| `CurveTests.DeCasteljauCubic_SamplesPointFromEquivalentCubicFormula` | 3.1 |
| `CurveTests.SceneCurve_SamplesCurrentWorldPointsWithoutCallbacks(True,False)` | 3.1 + 3.2 |
| `CurveTests.DeCasteljauCubic_SamplesTangentFromFinalInterpolationSegment` | 3.5 |
| `CurveTests.SceneCurve_SamplesCurrentWorldPointsWithoutCallbacks(True,True)` | 3.5 + 3.6 |
| `ThrownAxeTests.Launch_DetachesAndEnablesPhysicsWhileIgnoringThrower` | 4.1 |
| `CurveTests.BernsteinQuadratic_SamplesPointFromEquivalentQuadraticFormula` | 6.1 |
| `CurveTests.BernsteinQuadratic_SamplesTangentFromDerivativeFormula` | 6.3 |
| `CurveTests.PowerBasisQuadratic_SamplesPointFromEquivalentQuadraticFormula` | 7.1 + 7.2 |
| `CurveTests.PowerBasisQuadratic_SamplesTangentFromDerivativeFormula` | 7.1 + 7.4 |

### Must stay green from the start

- `ThrownAxeTests.CatchPosition_TracksMovingHandAndHeldOffsetAfterLaunch`
- `ThrownAxeTests.AttachToHand_RestoresPoseAndSupportsAnotherThrow`
- `PlayerAssetTests.PlayerPrefab_HasCompleteAxeWiring`
- `PlayerAssetTests.MainGame_HasWiredPlayerAndOneRenderer`

These four cover catch position, attachment restoration, and asset wiring. None of them exercises a physical collision or sticking. They guard the supplied wiring through slice 8; polish after that may change them. Tuning values are yours to adjust.

Expected totals after complete slices: start **4 pass / 13 fail** → slice 1 **6 / 11** → slice 2 **8 / 9** → slice 3 **12 / 5** → slice 4 **13 / 4** → slice 5 **13 / 4** → slice 6 **15 / 2** → slice 7 **17 / 0** → slice 8 stays **17 / 0**. Scene tests require both the evaluator and its adapter. Your `ThrownAxe.OnCollisionEnter` changes in slice 4 must keep the Launch test and the four supplied green tests passing.

A fully green Test Runner proves the supplied math, scene sampling, and existing asset/lifecycle checks. It does not prove gizmo drawing, line drawing, follower integration, recall timing or motion, spin, collision or sticking, target filtering (**4.2**), or which algorithm your adapters and line actually use: equivalent curves give equal values. Use the route's scene checks and implementation checks as well.
