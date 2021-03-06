import {expect} from 'chai';
import loginPage from '../../../Page objects/Login.page';
import {Guid} from 'guid-typescript';
import myEformsPage from '../../../Page objects/MyEforms.page';
import rentableItemsPage from '../../../Page objects/rentableitem-general/rentable-item-RentableItem.page';
import pluginsPage from '../../rentableitem-settings/application-settings.plugins.page';
import contractsPage from '../../../Page objects/rentableitem-general/rentable-item-Contract.page';
import customersPage from '../../../Page objects/Customers/Customers.page';
import customersModalPage from '../../../Page objects/Customers/CustomersModal.page';

describe('Rentable Items - Contracts - edit', function () {
  before(function () {
    loginPage.open('/auth');
    loginPage.login();
  });
  it('should go to customers page', function () {
    customersPage.goToCustomersPage();
  });
  it('should create multiple customers', function () {
    customersPage.newCustomerBtn.click();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    const customerObject2 = {
      createdBy: 'John Smith',
      customerNo: '2',
      contactPerson: 'Jack Black',
      companyName: 'Bents bjelker',
      companyAddress: 'ABC Street 23',
      zipCode: '0215521',
      cityName: 'Odense',
      phone: '1231245',
      email: 'high@user.com',
      eanCode: '22221185',
      vatNumber: '79485641',
      countryCode: 'DK',
      cadastralNumber: 'eal10230',
      propertyNumber: 1235,
      apartmentNumber: 52,
      completionYear: 1960,
      floorsWithLivingSpace: 3
    };
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    customersModalPage.createCustomer(customerObject2);
  });
  it('should create rentable item with all parameters', function () {
    rentableItemsPage.goToRentableItemsPage();
    const date2 = loginPage.randomInt(12, 24);
    const brand2 = 'Bosch';
    const model2 = 'Boremaskine';
    const serialNumber2 = '89798h3ffr23';
    const vinNumber2 = '8934r9hure893t';
    const plateNumber2 = '3f9wef0923r9uwf';
    const eForm = 'Number 1';
    rentableItemsPage.createRentableItem(brand2, model2, date2, eForm, serialNumber2, vinNumber2, plateNumber2);
  });
  it('should create contract', function () {
    loginPage.open('/');
    contractsPage.rentableItemDropdown();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    contractsPage.rentableItemDropdownItemName('Kontrakter').click();
    $('#contractCreateBtn').waitForDisplayed({timeout: 20000});
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    const date1 = loginPage.randomInt(12, 24);
    const date2 = loginPage.randomInt(12, 24);
    const contractNr = Math.floor((Math.random() * 100) + 1);
    const rentableItemName = 'Apple';
    const expectrentableItemName = 'Apple - MacBook - gfoijwf235 - ihu3t98wrgio34t - 9et9w90342wgr';
    const customerName = 'Oles olie';
    const expectcustomerName = 'Oles olie - Samantha Black - ABC Street 22 - Odense - 123124';
    contractsPage.createContract(date1, date2, contractNr, customerName, rentableItemName, expectcustomerName, expectrentableItemName);
    const contract = contractsPage.getFirstContractObject();
    expect(contract.contractNumber).equal(contractNr);
    expect($('#contractCustomer').getText()).equal('Oles olie\nSamantha Black');
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
  });
  it('should edit contract', function () {
    loginPage.open('/');
    contractsPage.rentableItemDropdown();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    contractsPage.rentableItemDropdownItemName('Kontrakter').click();
    $('#contractCreateBtn').waitForDisplayed({timeout: 20000});
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    const newStartDate = loginPage.randomInt(12, 24);
    const newEndDate = loginPage.randomInt(12, 24);
    const newContractNumber = Math.floor((Math.random() * 28) + 3);
    const newRentableItem = 'Bosch';
    const expectnewRentableItem = 'Bosch - Boremaskine - 89798h3ffr23 - 8934r9hure893t - 3f9wef0923r9uwf';
    const newCustomer = 'Bents bjelker';
    const expectnewCustomer = 'Bents bjelker - Jack Black - ABC Street 23 - Odense - 1231245';
    contractsPage.editContract(newStartDate, newEndDate, newContractNumber, newCustomer, newRentableItem, expectnewCustomer, expectnewRentableItem);
    loginPage.open('/');
    contractsPage.rentableItemDropdown();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    contractsPage.rentableItemDropdownItemName('Kontrakter').click();
    $('#contractCreateBtn').waitForDisplayed({timeout: 20000});
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    const contract = contractsPage.getFirstContractObject();
    expect(contract.contractNumber).equal(newContractNumber);
    expect($('#contractCustomer').getText()).equal('Bents bjelker\nJack Black');
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
  });
  it('should NOT edit contract', function () {
    loginPage.open('/');
    contractsPage.rentableItemDropdown();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    contractsPage.rentableItemDropdownItemName('Kontrakter').click();
    $('#contractCreateBtn').waitForDisplayed({timeout: 20000});
    const oldContract = contractsPage.getFirstContractObject();
    const newStartDate = loginPage.randomInt(12, 24);
    const newEndDate = loginPage.randomInt(12, 24);
    const newContractNumber = Math.floor((Math.random() * 28) + 3);
    const newRentableItem = 'Bosch';
    const expectnewRentableItem = 'Bosch - Boremaskine - 89798h3ffr23 - 8934r9hure893t - 3f9wef0923r9uwf';
    const newCustomer = 'Oles olie';
    const expectcustomerName = 'Oles olie - Samantha Black - ABC Street 22 - Odense - 123124';
    contractsPage.editContractCancel(newStartDate, newEndDate, newContractNumber, newCustomer, newRentableItem, expectcustomerName, expectnewRentableItem);
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    loginPage.open('/');
    contractsPage.rentableItemDropdown();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    contractsPage.rentableItemDropdownItemName('Kontrakter').click();
    $('#contractCreateBtn').waitForDisplayed({timeout: 20000});
    const contract = contractsPage.getFirstContractObject();
    expect($('#contractCustomer').getText()).equal('Bents bjelker\nJack Black');
    expect(oldContract.contractNumber).equal(contract.contractNumber);
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
  });
  it('should remove 1 rentable item', function () {
    loginPage.open('/');
    contractsPage.rentableItemDropdown();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    contractsPage.rentableItemDropdownItemName('Kontrakter').click();
    $('#contractCreateBtn').waitForDisplayed({timeout: 20000});
    contractsPage.editContractDeleteRentableItem();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    const contract = contractsPage.getFirstContractObject();
    contract.editBtn.click();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    expect(contractsPage.rentableItemListonContract).equal(1);
    contractsPage.contractEditCancelBtn.click();
    $('#spinner-animation').waitForDisplayed({timeout: 90000, reverse: true});
    contractsPage.cleanup();
  });
});
