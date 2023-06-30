import { test, expect } from '@playwright/test';
import { environment } from '../environment';

test('Submitting secret results in secret creation response', async ({page}) => {
  await page.goto(environment.frontendUrl);
  const textBox = await page.locator('//*[@id="secretText"]');
  const submitButton = await page.locator('//*[@id="submit"]');
  
  await textBox.fill('Hello, world!');
  await submitButton.click();

  const successMessage = await page.getByText('Secret received successfully. Use the link below to access the secret:');
  const secretUrl = await page.inputValue('input#secretUrl');

  await expect(successMessage).toBeTruthy();
  await expect(secretUrl).toBeTruthy();
});

test('Secret can be retrieved after it is created', async ({page}) => {
  const secretMessage = "Major Tom to ground control";
  await page.goto(environment.frontendUrl);
  const textBox = await page.locator('//*[@id="secretText"]');
  const submitButton = await page.locator('//*[@id="submit"]');
  
  await textBox.fill(secretMessage);
  await submitButton.click();
  const secretUrl = await page.inputValue('input#secretUrl');

  await page.goto(secretUrl);

  const secretRetrieve = await page.inputValue('textarea#secretMessage', {timeout: 5000});
  await expect(secretRetrieve).toBe(secretMessage);
});

test('Accessing the secret twice results in error', async ({page}) => {
  const secretMessage = "Major Tom to ground control";
  await page.goto(environment.frontendUrl);
  const textBox = await page.locator('//*[@id="secretText"]');
  const submitButton = await page.locator('//*[@id="submit"]');
  
  await textBox.fill(secretMessage);
  await submitButton.click();
  const secretUrl = await page.inputValue('input#secretUrl');

  await page.goto(secretUrl);

  const secretRetrieve1 = await page.locator('//*[@id="secretMessage"]');
  await expect(await secretRetrieve1.count()).toBe(1);

  await page.reload();

  const secretRetrieve2 = await page.locator('//*[@id="secretMessage"]');
  await expect(await secretRetrieve2.count()).toBe(0);

  await expect(await page.getByText("Either the secret not found, has expired, or has already been recovered – or your key was invalid.")).toBeTruthy();
});
