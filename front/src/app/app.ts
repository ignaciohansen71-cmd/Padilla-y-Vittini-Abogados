import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ContactanosComponent } from './contactanos/contactanos.component';
import { InicioComponent } from './inicio/inicio.component';
import { QuienesSomosComponent } from './quienes-somos/quienes-somos.component';
import { ServiciosComponent } from './servicios/servicios.component';

@Component({
  selector: 'app-root',
  imports: [
    RouterLink,
    InicioComponent,
    ServiciosComponent,
    QuienesSomosComponent,
    ContactanosComponent,
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly currentYear = new Date().getFullYear();
  protected readonly navigation = [
    { label: 'Inicio', fragment: 'inicio' },
    { label: 'Servicios', fragment: 'servicios' },
    { label: 'Quienes somos', fragment: 'quienes-somos' },
    { label: 'Contactanos', fragment: 'contactanos' },
  ];
}
