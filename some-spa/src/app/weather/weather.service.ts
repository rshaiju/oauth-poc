import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Weather } from './weather';
import { catchError, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class WeatherService {

  weatherUrl="https://localhost:7284/WeatherForecast";


  constructor(private http: HttpClient) {
  }

   getWeather():Observable<Weather[]>{
      return this.http.get<Weather[]>(this.weatherUrl);
   }
}
