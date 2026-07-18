import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-contactanos',
  imports: [ReactiveFormsModule],
  templateUrl: './contactanos.component.html',
  styleUrl: './contactanos.component.scss',
})
export class ContactanosComponent {
  private readonly formBuilder = inject(FormBuilder);
  protected submitted = false;

  protected readonly contactForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    phone: [''],
    service: ['', Validators.required],
    message: ['', [Validators.required, Validators.minLength(10)]],
  });

  protected submit(): void {
    this.submitted = true;
    this.contactForm.markAllAsTouched();
  }
}
