import React, { useState } from 'react';
import axios from 'axios';
import Snackbar from '@mui/material/Snackbar';
import Alert from '@mui/material/Alert';
import { useTranslation } from 'react-i18next';
import ButtonGroup from '@mui/material/ButtonGroup';
import Button from '@mui/material/Button';

function App() {
  const [data, setData] = useState({});
  const [location, setLocation] = useState('');
  const [error, setError] = useState('');
  const [open, setOpen] = useState(false);
  const { t, i18n } = useTranslation();
  const [lang, setLang] = useState('');

  const handleClose = () => {
    setOpen(false);
  };

  const url = `https://localhost:5000/api/weather/current?city=${location}&lang=${lang}`;

  const searchLocation = (event) => {
    if (event.key === 'Enter') {
      axios
        .get(url)
        .then((response) => {
          setData(response.data);
          console.log(response.data);
        })
        .catch((error) => {
          setOpen(true);
          setError(error.response.data);
          console.log(error.response.data);
        });
      setLocation('');
    }
  };

  return (
    <div className='app'>
      <div className='search'>
        <input
          value={location}
          onChange={(event) => setLocation(event.target.value)}
          onKeyDown={searchLocation}
          placeholder={t('Enter location')}
          type='text'
        />
      </div>
      <ButtonGroup
        disableElevation
        variant='contained'
        aria-label='Disabled elevation buttons'
      >
        <Button
          onClick={() => {
            i18n.changeLanguage('ru');
            setLang('ru');
          }}
          type='submit'
        >
          RU
        </Button>
        <Button
          onClick={() => {
            i18n.changeLanguage('en');
            setLang('en');
          }}
          type='submit'
        >
          EN
        </Button>
      </ButtonGroup>
      {error && (
        <Snackbar
          anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
          open={open}
          autoHideDuration={6000}
          onClose={handleClose}
        >
          <Alert onClose={handleClose} severity='info' sx={{ width: '100%' }}>
            {error.Messages[0]}
          </Alert>
        </Snackbar>
      )}
      <div className='container'>
        <div className='top'>
          <div className='location'>
            <p>{data.name}</p>
          </div>
          <div className='temp'>
            {data.temp ? <h1>{data.temp}°C</h1> : null}
          </div>
          <div className='description'>
            {data.description ? <p>{data.description}</p> : null}
          </div>
        </div>
        {data.name !== undefined && (
          <div className='bottom'>
            <div className='feels'>
              <p className='bold'>{data.feelsLike}°C</p>
              <p>{t('Feels Like')}</p>
            </div>
            <div className='humidity'>
              <p className='bold'>{data.humidity} %</p>
              <p>{t('Humidity')}</p>
            </div>
            <div className='wind'>
              <p className='bold'>
                {data.windSpeedKph} {t('km/h')}
              </p>
              <p>{t('Wind Speed')}</p>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default App;
