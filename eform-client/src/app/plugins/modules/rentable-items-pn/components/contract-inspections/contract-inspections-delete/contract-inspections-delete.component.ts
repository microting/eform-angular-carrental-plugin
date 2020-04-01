import {Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';
import {ContractInspectionsService} from '../../../services';
import {ContractInspectionModel, ContractModel} from '../../../models';

@Component({
  selector: 'app-contract-inspections-delete',
  templateUrl: './contract-inspections-delete.component.html',
  styleUrls: ['./contract-inspections-delete.component.scss']
})
export class ContractInspectionsDeleteComponent implements OnInit {
  @ViewChild('frame', {static: false}) frame;
  @Output() onContractInspectionDeleted: EventEmitter<void> = new EventEmitter<void>();
  selectedContractInspectionModel: ContractInspectionModel = new ContractInspectionModel();
  spinnerStatus = false;
  constructor(private contractInspectionService: ContractInspectionsService) { }

  ngOnInit() {
  }
  show(contractInspectionModel: ContractInspectionModel) {
    this.selectedContractInspectionModel = contractInspectionModel;
    this.frame.show();
  }
  deleteContractInspection() {
    this.spinnerStatus = true;
    this.contractInspectionService.deleteInspection(this.selectedContractInspectionModel.id).subscribe((data) => {
      if (data && data.success) {
        this.onContractInspectionDeleted.emit();
        this.frame.hide();
      } this.spinnerStatus = false;
    });
  }
}
