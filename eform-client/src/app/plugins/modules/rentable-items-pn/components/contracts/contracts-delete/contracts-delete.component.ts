import {Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {ContractModel} from '../../../models';
import {ContractsService} from '../../../services';

@Component({
  selector: 'app-contract-delete',
  templateUrl: './contracts-delete.component.html',
  styleUrls: ['./contracts-delete.component.scss']
})
export class ContractsDeleteComponent implements OnInit {
  @ViewChild('frame') frame;
  @Output() onContractDeleted: EventEmitter<void> = new EventEmitter<void>();
  selectedContractModel: ContractModel = new ContractModel();
  constructor(private contractsService: ContractsService) { }

  ngOnInit() {
  }
  show(contractModel: ContractModel) {
    this.selectedContractModel = contractModel;
    this.frame.show();
  }
  deleteContract() {
    this.contractsService.deleteContract(this.selectedContractModel.id).subscribe((data) => {
      if (data && data.success) {
        this.onContractDeleted.emit();
        this.frame.hide();
      }
    });
  }
}
