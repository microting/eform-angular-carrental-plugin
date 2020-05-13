import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {BaseService} from '../../../../common/services/base.service';
import {ContractModel, ContractsRequestModel, CustomerModel, CustomerRequestModel} from '../models';
import {Observable} from 'rxjs';
import {OperationDataResult, OperationResult} from '../../../../common/models';

const ContractMethods = {
  Contracts: '/api/rentable-items-pn/contracts',
  CreateContract: '/api/rentable-items-pn/contracts',
  UpdateContract: '/api/rentable-items-pn/contracts',
  DeleteContract: '/api/rentable-items-pn/contracts',
  Customers: 'api/customers-pn/customers'
};

@Injectable()
export class ContractsService extends BaseService {

  constructor(private _http: HttpClient, router: Router, toastrService: ToastrService) {
    super(_http, router, toastrService);
  }

  getAllContracts(model: ContractsRequestModel): Observable<any> {
    return this.get(ContractMethods.Contracts, model);
  }

  createContract(model: ContractModel): Observable<any> {
    return this.post(ContractMethods.CreateContract, model);
  }

  updateContract(model: ContractModel): Observable<any> {
    return this.post(ContractMethods.UpdateContract, model);
  }

  deleteContract(contractId: number): Observable<OperationResult> {
    return this.delete(ContractMethods.DeleteContract + '/' + contractId);
  }
  getCustomer(model: CustomerRequestModel): Observable<any> {
    return this.post(ContractMethods.Customers, model);
  }
  getSingleCustomer(customerId: number): Observable<OperationDataResult<CustomerModel>> {
    return this.get(ContractMethods.Customers + '/' + customerId);
  }
}
