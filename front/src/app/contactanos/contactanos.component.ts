import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-contactanos',
  imports: [ReactiveFormsModule],
  templateUrl: './contactanos.component.html',
  styleUrl: './contactanos.component.scss',
})
export class ContactanosComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly http = inject(HttpClient);
  protected readonly submissionState = signal<'idle' | 'sending' | 'success' | 'error'>('idle');
  protected readonly errorMessage = signal('');

  protected readonly contactForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    phone: [''],
    service: ['', Validators.required],
    message: ['', [Validators.required, Validators.minLength(10)]],
    website: [''],
  });

  protected submit(): void {
    this.contactForm.markAllAsTouched();

    if (this.contactForm.invalid || this.submissionState() === 'sending') {
      return;
    }

    this.submissionState.set('sending');
    this.errorMessage.set('');

    this.http.post<{ message: string }>('/api/contact', this.contactForm.getRawValue()).subscribe({
      next: () => {
        this.submissionState.set('success');
        this.contactForm.reset();
      },
      error: (error: HttpErrorResponse) => {
        this.submissionState.set('error');
        this.errorMessage.set(
          error.status === 429
            ? 'Has realizado varios intentos. Espera unos minutos antes de volver a enviar.'
            : 'No pudimos enviar tu consulta. Intenta nuevamente o escribenos directamente.',
        );
      },
    });
  }

  protected resetForm(): void {
    this.submissionState.set('idle');
    this.errorMessage.set('');
  }
}
