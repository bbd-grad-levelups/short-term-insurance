import { ChangeDetectorRef, Component, ViewChild } from '@angular/core';
import { PersonaTableComponent } from '../../components/persona-table/persona-table.component';
import {MatTableDataSource, MatTableModule} from "@angular/material/table";
import { MatPaginator, MatPaginatorIntl, MatPaginatorModule } from '@angular/material/paginator';
import { InsuranceService } from '../../services/insurance.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Persona } from '../../models/persona.model';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-persona-page',
  standalone: true,
  imports: [
    PersonaTableComponent,
    MatPaginatorModule,
    MatIcon,
    MatTableModule
  ],
  templateUrl: './persona-page.component.html',
  styleUrl: './persona-page.component.css'
})
export class PersonaPageComponent {

  displayedColumns: string[] = ['Persona'];

  dataSource = new MatTableDataSource<Persona>([]);
  @ViewChild(MatPaginator) paginator: MatPaginator = new MatPaginator(
    new MatPaginatorIntl(), ChangeDetectorRef.prototype
  );

  isMobile: boolean = false;
  error: boolean = false;
  loading: boolean = true;
  isLastPage: boolean = false;
  page: number = 1;

  constructor(
    private insuranceService: InsuranceService,
    private snackBar: MatSnackBar 
  ) {}

  ngOnInit() {
    this.getCashFlows();
  }

  getCashFlows(nextPage: boolean = false) {
    this.error = false;
    this.insuranceService.getPersonas(this.page)
      .subscribe({
        next: response => {
          if (response.success) {
            this.isLastPage = !response.data.length;
            if (nextPage && this.isLastPage) {
              this.page--;
              this.snackBar.open('On Last Page.', 'Ok', {"duration": 4000});
              return;
            }

            console.log(response.data);

            this.dataSource = new MatTableDataSource<Persona>(response.data);
            this.loading = false;

          } else {
            this.error = true;
          }
        },
        error: () => {
          this.error = true;
        },
      })
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  previousPage() {
    if (this.page === 1) {
      this.snackBar.open('On First Page.', 'Ok', {"duration": 4000});
      return;
    }

    this.page--;
    this.getCashFlows()
  }

  nextPage() {
    this.page++;
    this.getCashFlows(true)
  }
}
