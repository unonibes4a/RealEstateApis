import React, { useState, useEffect, useMemo } from 'react';

const RealEstateApp = () => {
  const [apiUrl] = useState('http://localhost:5000/api/realestate');
  const [properties, setProperties] = useState([]);
  const [status, setStatus] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [filters, setFilters] = useState({
    name: '',
    address: '',
    minPrice: '',
    maxPrice: '',
    year: ''
  });

  const clearMessages = () => {
    setError(null);
    setSuccess(null);
  };

  const formatPrice = (price) => {
    return new Intl.NumberFormat('es-CO').format(price);
  };

  const loadStatus = async () => {
    clearMessages();
    setLoading(true);

    try {
      const response = await fetch(`${apiUrl}/status`);
      const data = await response.json();

      if (data.success) {
        setStatus(data);
        setSuccess('Estado cargado correctamente');
      } else {
        setError(data.message || 'Error cargando estado');
      }
    } catch (err) {
      setError(`Error de conexión: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  const loadProperties = async () => {
    clearMessages();
    setLoading(true);

    try {
      const response = await fetch(`${apiUrl}/properties`);
      const data = await response.json();

      if (data.success) {
        setProperties(data.data || []);
        setSuccess(`${data.data?.length || 0} propiedades cargadas`);
      } else {
        setError(data.message || 'Error cargando propiedades');
      }
    } catch (err) {
      setError(`Error de conexión: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  const filterProperties = async () => {
    clearMessages();
    setLoading(true);

    try {
      const params = new URLSearchParams();

      if (filters.name) params.append('name', filters.name);
      if (filters.address) params.append('address', filters.address);
      if (filters.minPrice) params.append('minPrice', filters.minPrice);
      if (filters.maxPrice) params.append('maxPrice', filters.maxPrice);
      if (filters.year) params.append('year', filters.year);

      const queryString = params.toString();
      const url = `${apiUrl}/properties${queryString ? '?' + queryString : ''}`;

      const response = await fetch(url);
      const data = await response.json();

      if (data.success) {
        setProperties(data.data || []);
        setSuccess(`${data.data?.length || 0} propiedades encontradas`);
      } else {
        setError(data.message || 'Error filtrando propiedades');
      }
    } catch (err) {
      setError(`Error de conexión: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  const seedData = async () => {
    clearMessages();
    setLoading(true);

    try {
      const response = await fetch(`${apiUrl}/seed-data`, {
        method: 'POST'
      });
      const data = await response.json();

      if (data.success) {
        setSuccess('Datos de prueba creados correctamente');
        setTimeout(() => {
          loadStatus();
          loadProperties();
        }, 1000);
      } else {
        setError(data.message || 'Error creando datos');
      }
    } catch (err) {
      setError(`Error de conexión: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  const getExpensiveProperties = async () => {
    clearMessages();
    setLoading(true);

    try {
      const response = await fetch(`${apiUrl}/search/expensive?limit=5`);
      const data = await response.json();

      if (data.success) {
        setProperties(data.data || []);
        setSuccess(`${data.data?.length || 0} propiedades más caras`);
      } else {
        setError(data.message || 'Error obteniendo propiedades caras');
      }
    } catch (err) {
      setError(`Error de conexión: ${err.message}`);
    } finally {
      setLoading(false);
    }
  };

  const handleFilterChange = (field, value) => {
    setFilters(prev => ({ ...prev, [field]: value }));
  };

  const memoizedProperties = useMemo(() => properties, [properties]);
  const memoizedStatus = useMemo(() => status, [status]);

  useEffect(() => {
    loadStatus();
  }, []);

  useEffect(() => {
    if (success || error) {
      const timeout = setTimeout(() => {
        clearMessages();
      }, 5000);
      return () => clearTimeout(timeout);
    }
  }, [success, error]);

  return (
    <>
      <style>{`
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #eee 0%, #ccc 100%);
            min-height: 100vh;
            color: #333;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }

        .header {
            text-align: center;
            color: #333;
            margin-bottom: 30px;
        }

        .header h1 {
            font-size: 2.5rem;
            margin-bottom: 10px;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.1);
        }

        .status-card {
            background: white;
            border-radius: 15px;
            padding: 20px;
            margin-bottom: 20px;
            box-shadow: 0 8px 32px rgba(0,0,0,0.05);
            backdrop-filter: blur(10px);
        }

        .controls {
            display: flex;
            gap: 15px;
            flex-wrap: wrap;
            margin-bottom: 20px;
        }

        .btn {
            padding: 12px 24px;
            border: none;
            border-radius: 25px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 600;
            transition: all 0.3s ease;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .btn-primary {
            background: linear-gradient(45deg, #666, #333);
            color: white;
        }

        .btn-success {
            background: linear-gradient(45deg, #555, #222);
            color: white;
        }

        .btn-warning {
            background: linear-gradient(45deg, #777, #444);
            color: white;
        }

        .btn-info {
            background: linear-gradient(45deg, #888, #555);
            color: white;
        }

        .btn:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.1);
        }

        .filters {
            background: rgba(255,255,255,0.95);
            border-radius: 15px;
            padding: 20px;
            margin-bottom: 20px;
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
        }

        .input-group {
            display: flex;
            flex-direction: column;
        }

        .input-group label {
            font-weight: 600;
            margin-bottom: 5px;
            color: #555;
        }

        .input-group input, .input-group select {
            padding: 10px 15px;
            border: 2px solid #e1e1e1;
            border-radius: 10px;
            font-size: 14px;
            transition: border-color 0.3s ease;
        }

        .input-group input:focus, .input-group select:focus {
            outline: none;
            border-color: #666;
        }

        .properties-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
            gap: 20px;
        }

        .property-card {
            background: white;
            border-radius: 15px;
            overflow: hidden;
            box-shadow: 0 8px 32px rgba(0,0,0,0.05);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }

        .property-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 15px 40px rgba(0,0,0,0.1);
        }

        .property-header {
            background: linear-gradient(45deg, #666, #333);
            color: white;
            padding: 15px 20px;
            position: relative;
        }

        .property-header::after {
            content: '';
            position: absolute;
            top: 0;
            right: 0;
            width: 0;
            height: 0;
            border-style: solid;
            border-width: 0 0 50px 50px;
            border-color: transparent transparent rgba(255,255,255,0.1) transparent;
        }

        .property-name {
            font-size: 1.2rem;
            font-weight: bold;
            margin-bottom: 5px;
        }

        .property-code {
            font-size: 0.9rem;
            opacity: 0.9;
        }

        .property-body {
            padding: 20px;
        }

        .property-price {
            font-size: 1.5rem;
            font-weight: bold;
            color: #444;
            margin-bottom: 10px;
        }

        .property-details {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 10px;
            margin-bottom: 15px;
        }

        .property-detail {
            display: flex;
            align-items: center;
            font-size: 0.9rem;
            color: #666;
        }

        .property-detail strong {
            margin-right: 5px;
        }

        .property-address {
            background: #f8f8f8;
            padding: 10px;
            border-radius: 8px;
            font-size: 0.9rem;
            color: #555;
            border-left: 4px solid #666;
        }

        .loading {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 200px;
            font-size: 1.2rem;
            color: #333;
        }

        .spinner {
            border: 4px solid rgba(0,0,0,0.3);
            border-top: 4px solid #333;
            border-radius: 50%;
            width: 40px;
            height: 40px;
            animation: spin 1s linear infinite;
            margin-right: 15px;
        }

        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }

        .error {
            background: #ffaaaa;
            color: #a00;
            padding: 15px;
            border-radius: 10px;
            margin: 20px 0;
            text-align: center;
        }

        .success {
            background: #aaffaa;
            color: #0a0;
            padding: 15px;
            border-radius: 10px;
            margin: 20px 0;
            text-align: center;
        }

        .stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin-bottom: 20px;
        }

        .stat-card {
            background: rgba(255,255,255,0.95);
            padding: 20px;
            border-radius: 15px;
            text-align: center;
        }

        .stat-number {
            font-size: 2rem;
            font-weight: bold;
            color: #666;
        }

        .stat-label {
            color: #666;
            margin-top: 5px;
        }

        @media (max-width: 768px) {
            .container {
                padding: 10px;
            }
            
            .header h1 {
                font-size: 1.8rem;
            }
            
            .properties-grid {
                grid-template-columns: 1fr;
            }
            
            .controls {
                justify-content: center;
            }
            
            .filters {
                grid-template-columns: 1fr;
            }
        }
      `}</style>
      
      <div className="container">
        <div className="header">
          <h1>Real Estate API</h1>
          <p>Gestión de propiedades con React.js y API REST</p>
        </div>

        {memoizedStatus && (
          <div className="status-card">
            <h3>Estado de la Base de Datos</h3>
            <div className="stats">
              <div className="stat-card">
                <div className="stat-number">{memoizedStatus.collections?.properties || 0}</div>
                <div className="stat-label">Propiedades</div>
              </div>
              <div className="stat-card">
                <div className="stat-number">{memoizedStatus.collections?.owners || 0}</div>
                <div className="stat-label">Propietarios</div>
              </div>
              <div className="stat-card">
                <div className="stat-number">{memoizedStatus.collections?.propertyImages || 0}</div>
                <div className="stat-label">Imágenes</div>
              </div>
              <div className="stat-card">
                <div className="stat-number">{memoizedStatus.totalRecords || 0}</div>
                <div className="stat-label">Total Registros</div>
              </div>
            </div>
          </div>
        )}

        <div className="controls">
          <button className="btn btn-primary" onClick={loadProperties}>Cargar Propiedades</button>
          <button className="btn btn-success" onClick={loadStatus}>Estado BD</button>
          <button className="btn btn-warning" onClick={seedData}>Crear Datos</button>
          <button className="btn btn-info" onClick={getExpensiveProperties}>Más Caras</button>
        </div>

        <div className="filters">
          <div className="input-group">
            <label>Nombre</label>
            <input 
              value={filters.name} 
              onChange={(e) => handleFilterChange('name', e.target.value)}
              type="text" 
              placeholder="Buscar por nombre..."
            />
          </div>
          <div className="input-group">
            <label>Dirección</label>
            <input 
              value={filters.address} 
              onChange={(e) => handleFilterChange('address', e.target.value)}
              type="text" 
              placeholder="Buscar por dirección..."
            />
          </div>
          <div className="input-group">
            <label>Precio Mínimo</label>
            <input 
              value={filters.minPrice} 
              onChange={(e) => handleFilterChange('minPrice', e.target.value)}
              type="number" 
              placeholder="Ej: 100000000"
            />
          </div>
          <div className="input-group">
            <label>Precio Máximo</label>
            <input 
              value={filters.maxPrice} 
              onChange={(e) => handleFilterChange('maxPrice', e.target.value)}
              type="number" 
              placeholder="Ej: 500000000"
            />
          </div>
          <div className="input-group">
            <label>Año</label>
            <input 
              value={filters.year} 
              onChange={(e) => handleFilterChange('year', e.target.value)}
              type="number" 
              placeholder="Ej: 2020"
            />
          </div>
          <div className="input-group">
            <button className="btn btn-primary" onClick={filterProperties} style={{marginTop: '25px'}}>Filtrar</button>
          </div>
        </div>

        {loading && (
          <div className="loading">
            <div className="spinner"></div>
            Cargando...
          </div>
        )}

        {error && (
          <div className="error">
            {error}
          </div>
        )}

        {success && (
          <div className="success">
            {success}
          </div>
        )}

        {memoizedProperties.length > 0 && (
          <div className="properties-grid">
            {memoizedProperties.map((property) => (
              <div key={property.id} className="property-card">
                <div className="property-header">
                  <div className="property-name">{property.name}</div>
                  <div className="property-code">Código: {property.codeInternal}</div>
                </div>
                <div className="property-body">
                  <div className="property-price">
                    ${formatPrice(property.price)}
                  </div>
                  <div className="property-details">
                    <div className="property-detail">
                      <strong>Año:</strong> {property.year}
                    </div>
                    <div className="property-detail">
                      <strong>Owner:</strong> {property.idOwner}
                    </div>
                  </div>
                  <div className="property-address">
                    <strong>Dirección:</strong> {property.address}
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {!loading && memoizedProperties.length === 0 && !error && (
          <div className="status-card" style={{textAlign: 'center'}}>
            <h3>No se encontraron propiedades</h3>
            <p>Intenta cargar los datos o ajustar los filtros</p>
          </div>
        )}
      </div>
    </>
  );
};

export default RealEstateApp;