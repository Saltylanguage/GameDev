# GLJP third-shop route - November 6, 2026

Local uncommitted work on the current branch. GLJP remains incomplete. No push.

The journey session, payout ledger, simulation preview, and portable replay now derive their length from the resolved route. A shop opens only after a completed simulation whose next route node is a shop. The final simulation still settles and ends the journey. The authored starter asset remains Simulation, Shop, Simulation, Shop, Simulation. Rebuilding that three-simulation shape produces the same fingerprint.

`FieldDataJourneyDefinition.WithSimulationCount` builds a test route only. It is not an authored production layout. A four-simulation copy buys +1 and 1.5× at shop 1, replaces that with 1.75× at shop 2, and replaces that with 2× at shop 3. The 2× tier stays unavailable until the previous tier is already owned. Later payouts use the purchased tier: 300, then 350, then 400 Field Data for the stable 100-Hare fixture. The route then completes 24 rounds and can discard the remaining balance.

Focused EditMode `FieldDataJourney` passed 46/46 after the Editor recompiled (`artifacts/unity-tests-20261106-143107`), including the new third-shop test and the existing two-shop route. The full EditMode and PlayMode suites were not rerun. The authored seed-10100 survival failure remains separate ecological evidence. Exact production shop placement, and behavior for route kinds other than Simulation and Shop, remain open.
