import { ChangeDetectorRef, Component, ViewChild } from '@angular/core';
import { MatTableDataSource, MatTableModule } from "@angular/material/table";
import { MatPaginator, MatPaginatorIntl, MatPaginatorModule } from '@angular/material/paginator';
import { InsuranceService } from '../../services/insurance.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Persona } from '../../models/persona.model';
import { MatIcon } from '@angular/material/icon';
import { PreventDoubleClick } from '../../directives/prevent-double-click.directive';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-persona-page',
  standalone: true,
  imports: [
    PreventDoubleClick,
    MatPaginatorModule,
    MatIcon,
    MatProgressSpinnerModule,
    MatTableModule
  ],
  templateUrl: './persona-page.component.html',
  styleUrl: './persona-page.component.css'
})
export class PersonaPageComponent {

  @ViewChild(MatPaginator) paginator: MatPaginator = new MatPaginator(
    new MatPaginatorIntl(), ChangeDetectorRef.prototype
  );

  dataSource: MatTableDataSource<Persona> = new MatTableDataSource<Persona>();

  displayedColumns: string[] = ['Persona'];
  error: boolean = false;
  loading: boolean = true;
  isLastPage: boolean = false;
  page: number = 1;

  constructor(
    private insuranceService: InsuranceService,
    private snackBar: MatSnackBar 
  ) {}

  ngOnInit() {
    this.getPersonaData();
  }

  getPersonaData(nextPage: boolean = false) {
    this.error = false;
    this.loading = true;
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
    this.getPersonaData()
  }

  nextPage() {
    this.page++;
    this.getPersonaData(true)
  }
}
