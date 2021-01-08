import loginPage from '../../Page objects/Login.page';
import myEformsPage from '../../Page objects/MyEforms.page';
import pluginPage from '../../Page objects/Plugin.page';

import {expect} from 'chai';
import pluginsPage from './application-settings.plugins.page';
import inspectionsPage from '../../Page objects/rentableitem-general/rentable-item-Inspection.page';
import {Guid} from 'guid-typescript';
import deviceUsersPage from '../../Page objects/DeviceUsers.page';
import rentableItemsSettingsPage from '../../Page objects/rentableitem-general/rentable-item-Settings.page';

describe('Application settings page - site header section', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
  });
  it('should go to plugin settings page', function () {

    myEformsPage.Navbar.advancedDropdownClick();
    myEformsPage.Navbar.clickOnSubMenuItem('Plugins');
    $('#plugin-name').waitForDisplayed({timeout: 50000});
    $('#spinner-animation').waitForDisplayed({timeout: 10000, reverse: true});

    const plugin = pluginsPage.getFirstPluginRowObj();

    if (plugin.name === 'Microting Rentable Items Plugin') {
      expect(plugin.name).equal('Microting Rentable Items Plugin');
    } else {
      expect(plugin.name).equal('Microting Customers Plugin');
    }
    expect(plugin.version).equal('1.0.0.0');

    const pluginTwo = pluginsPage.getSecondPluginRowObj();
    if (pluginTwo.name === 'Microting Rentable Items Plugin') {
      expect(pluginTwo.name).equal('Microting Rentable Items Plugin');
    } else {
      expect(pluginTwo.name).equal('Microting Customers Plugin');
    }
    expect(pluginTwo.version).equal('1.0.0.0');

  });
  it('should activate the customer plugin', function () {
    const plugin = pluginsPage.getFirstPluginRowObj();
    plugin.activateBtn.click();
    $('#pluginOKBtn').waitForDisplayed({timeout: 40000});
    pluginPage.pluginOKBtn.click();
    browser.pause(50000); // We need to wait 50 seconds for the plugin to create db etc.
    loginPage.open('/');

    loginPage.login();
    myEformsPage.Navbar.advancedDropdownClick();
    myEformsPage.Navbar.clickOnSubMenuItem('Plugins');
    $('#plugin-name').waitForDisplayed({timeout: 50000});
    $('#spinner-animation').waitForDisplayed({timeout: 10000, reverse: true});

    const secondPlugin = pluginsPage.getSecondPluginRowObj();
    expect(secondPlugin.version).equal('1.0.0.0');

    // pluginPage.pluginSettingsBtn.click();
    secondPlugin.activateBtn.click();
    $('#pluginOKBtn').waitForDisplayed({timeout: 40000});
    pluginPage.pluginOKBtn.click();
    browser.pause(50000); // We need to wait 50 seconds for the plugin to create db etc.
    loginPage.open('/');

    loginPage.login();
    myEformsPage.Navbar.advancedDropdownClick();
    myEformsPage.Navbar.clickOnSubMenuItem('Plugins');
    $('#plugin-name').waitForDisplayed({timeout: 50000});
    $('#spinner-animation').waitForDisplayed({timeout: 10000, reverse: true});

    const pluginToFind = pluginsPage.getFirstPluginRowObj();
    expect(pluginToFind.version).equal('1.0.0.0');
    $(`//*[contains(text(), 'Udlejning')]`).waitForDisplayed({timeout: 20000});
    $(`//*[contains(text(), 'Kunder')]`).waitForDisplayed({timeout: 20000});
  });

  it('should create eform', function () {
    loginPage.open('/');
    const label = 'Number 1';
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    inspectionsPage.createNewEform(label);
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
  });
  it('should create a new device user', function () {
    myEformsPage.Navbar.goToDeviceUsersPage();
    $('#newDeviceUserBtn').waitForDisplayed({timeout: 20000});
    const name = 'Alice';
    const surname = 'Springs';
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    deviceUsersPage.createNewDeviceUser(name, surname);
  });
  it('should add eForm and device user to settings', function () {
    myEformsPage.Navbar.goToDeviceUsersPage();
    const deviceUser = deviceUsersPage.getDeviceUser(1);
    const sdkSiteId = deviceUser.siteId.getText();
    myEformsPage.Navbar.advancedDropdownClick();
    myEformsPage.Navbar.clickOnSubMenuItem('Plugins');
    $('#plugin-name').waitForDisplayed({timeout: 50000});
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    const plugin = pluginsPage.getFirstPluginRowObj();
    const pluginTwo = pluginsPage.getSecondPluginRowObj();
    if (plugin.name === 'Microting Rentable Items Plugin') {
     plugin.settingsBtn.click();
    } else {
     pluginTwo.settingsBtn.click();
    }
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    rentableItemsSettingsPage.sdkSiteIdField.addValue(sdkSiteId);
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    rentableItemsSettingsPage.eFormSelector.addValue('Number 1');
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    inspectionsPage.selectOption('Number 1');
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    rentableItemsSettingsPage.saveBtn.click();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
  });
});
