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

  await expect(successMessage).toBeVisible();
  await expect(secretUrl).toContain(environment.frontendUrl);
});

test('Secret can be retrieved after it is created', async ({page}) => {
  const secretMessage = "Ground Control to Major Tom";
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
  const secretMessage = "Your circuit's dead, is something wrong?";
  await page.goto(environment.frontendUrl);
  const textBox = await page.locator('//*[@id="secretText"]');
  const submitButton = await page.locator('//*[@id="submit"]');
  
  await textBox.fill(secretMessage);
  await submitButton.click();
  const secretUrl = await page.inputValue('input#secretUrl');

  await page.goto(secretUrl);

  const secretRetrieve1 = await page.getByText(secretMessage);
  await expect(secretRetrieve1).toBeVisible();

  await page.goto(secretUrl);

  const errorMessage = await page.getByText("Either the secret not found, has expired, or has already been recovered – or your key was invalid.");
  const secretRetrieve2 = await page.getByText(secretMessage);

  await expect(errorMessage).toBeVisible();
  await expect(secretRetrieve2).not.toBeVisible();
});
