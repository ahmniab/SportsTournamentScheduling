import React from 'react';
import { Stadium } from '../../../types/types';

const ShowStadium: React.FC<{ stadium: Stadium }> = ({ stadium }) => {
  const img = stadium.logo || stadium.logoUrl;
  return (
    <div className="team-card">
      <div className="team-card-body">
        <div className="team-meta">
          <h3 className="team-name">{stadium.name}</h3>
        </div>
        {img && (
          <div className="team-image">
            <img src={img} alt={stadium.name} style={{ width: '100%' }}/>
          </div>
        )}
      </div>
    </div>
  );
};

export default ShowStadium;
