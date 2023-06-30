# End-to-end tests with Playwright

Run `npm install` to install the necessary components. Use the Node version referenced in `.nvmrc`.

## Prerequisites

- Must have the API and Database Docker containers running
- Must be running the SPA on port `4200`.

## Running end-to-end tests

To run an end-to-end test with Playwright, use the following command:

```bash
npx playwright test
```

If you want to run a trace on the tests (see screenshots of activity), run the following command:

```bash
npx playwright test --trace on
```

## Viewing reports

When a test fails, an HTML report will automatically open in your default browser. If all tests pass, you can view the HTML report using the following command:

```bash
npx playwright show-report
```
