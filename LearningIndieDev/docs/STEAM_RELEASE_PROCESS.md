# Steam Release Process

Valve's approval process mainly verifies that the game works and that its Steam store page accurately represents the version players will receive.

## Release steps

1. **Register with Steamworks**
   - Sign the Steam Distribution Agreement.
   - Submit identity, banking, and tax information.
   - Pay the US$100 Steam Direct fee for the game. The fee is recouped after the game reaches US$1,000 in adjusted Steam revenue.

2. **Create the Steam store page**
   - Add the description, screenshots, trailer, capsule artwork, system requirements, supported languages, features, content questionnaire, pricing, and release information.
   - Only advertise content and features that will exist in the launch build.
   - Submit the store page to Valve for review.

3. **Publish the Coming Soon page**
   - Once approved, publish the page so players can wishlist the game.
   - It must be publicly visible for at least two weeks before release.
   - Publishing it several months before launch is preferable for building wishlists.

4. **Prepare and upload the Steam build**
   - Configure depots and launch options.
   - Upload a near-final build through SteamPipe.
   - Test installation, launch, saving, uninstalling, controller support, achievements, Steam Cloud, and every operating system claimed on the store page.
   - Steamworks API integration is optional unless the game depends on particular Steam features.

5. **Complete both release checklists**
   - Complete the Store Presence checklist and the Game Build checklist.
   - Finish the content survey, legal information, pricing, executable configuration, and supported-feature declarations.
   - The store page must be submitted before the build can be submitted.

6. **Submit the build for Valve review**
   - Valve checks that the game launches correctly and includes the advertised features.
   - Review normally takes 3-5 business days, but allow at least 7 business days in case changes are requested.

7. **Address any feedback**
   - Common issues include incorrect launch configuration, missing redistributables, advertised but unavailable features, non-gameplay screenshots, and inaccurate controller or language support claims.

8. **Release the game**
   - Once the store page and build are approved and the two-week Coming Soon requirement has been met, release is controlled by the developer.
   - Use **Release App** in Steamworks at the chosen launch time.

## Suggested schedule

| Time before launch | Action |
| --- | --- |
| 3-6 months | Register with Steamworks and begin the store page. |
| 2-4 months | Publish the Coming Soon page and start collecting wishlists. |
| 3-4 weeks | Upload and thoroughly test the Steam build. |
| 1-2 weeks minimum | Submit the near-final build for review. |
| Launch day | Use **Release App** to publish the game. |

The game does not need to be complete before its store page is published. Opening the page relatively early provides time to build wishlists before launch.

## Official references

- [Steam Direct fee](https://partner.steamgames.com/doc/gettingstarted/appfee)
- [Valve review process](https://partner.steamgames.com/doc/store/review_process)
- [Steam release process](https://partner.steamgames.com/doc/store/releasing)

