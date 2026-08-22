# Bev Experimental Features: Question 1 Fixed-Contact Survival Lab

## Question

Does increasing Hare block make a Hare survive longer when every successful Fox hit is lethal?

## Test design

- Bev experimental opposed-roll combat was enabled.
- Each episode contained exactly one Fox and one Hare.
- The Fox and Hare were held in fixed contact.
- Movement, reproduction, feeding, metabolism, and population effects were removed from the test.
- The Fox made repeated attack opportunities until the first successful hit.
- Each block value from 0 through 10 was tested across 1,000 deterministic episodes.
- The measured value was the average number of Fox attack attempts the Hare survived before the first lethal hit.

## Result

| Hare block | Average attacks survived before death |
| ---: | ---: |
| 0 | 1.762 |
| 1 | 1.923 |
| 2 | 2.113 |
| 3 | 2.314 |
| 4 | 2.523 |
| 5 | 2.777 |
| 6 | 3.078 |
| 7 | 3.387 |
| 8 | 3.817 |
| 9 | 4.342 |
| 10 | 5.103 |

## Finding

Question 1 is answered: **yes, block makes a Hare survive longer before the lethal hit.**

The improvement is monotonic, but not linear. Block 10 survives about 2.9 times as many attack attempts as block 0 in this isolated contact test. That shape is expected for a defense stat that reduces the chance of being hit: each avoided attack creates another opportunity to avoid the next one.

This proves the local combat effect, not population fitness. The next question is whether those extra avoided hits translate into more lifetime, reproduction, or population stability once movement, reproduction, and ecology are restored.

## Verification

- EditMode: 156/156 passed.
- Artifact: `artifacts/unity-tests-20260821-215839/EditMode-results.xml`
- Test: `FixedContactSurvivalLabShowsHowBlockChangesAttacksUntilFirstLethalHit`
- Test source: `Assets/Tests/Runtime/SpeciesDomainTests.cs`
