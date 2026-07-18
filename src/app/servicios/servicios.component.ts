import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-servicios',
  imports: [RouterLink],
  templateUrl: './servicios.component.html',
  styleUrl: './servicios.component.scss',
})
export class ServiciosComponent {
  protected readonly services = [
    {
      icon: 'bi-briefcase',
      title: 'Derecho civil',
      text: 'Contratos, obligaciones, indemnizaciones y solucion de controversias entre personas y empresas.',
    },
    {
      icon: 'bi-building',
      title: 'Derecho corporativo',
      text: 'Asesoria permanente para sociedades, operaciones comerciales y toma de decisiones empresariales.',
    },
    {
      icon: 'bi-people',
      title: 'Derecho laboral',
      text: 'Orientacion preventiva y representacion en relaciones laborales, despidos y negociaciones.',
    },
    {
      icon: 'bi-house-door',
      title: 'Familia y patrimonio',
      text: 'Acompanamiento sensible y reservado en materias familiares, sucesorias y patrimoniales.',
    },
    {
      icon: 'bi-file-earmark-check',
      title: 'Contratos',
      text: 'Redaccion, revision y negociacion de acuerdos claros que protejan tus intereses.',
    },
    {
      icon: 'bi-bank',
      title: 'Litigios',
      text: 'Representacion estrategica en procedimientos judiciales y mecanismos alternativos de solucion.',
    },
  ];
}
