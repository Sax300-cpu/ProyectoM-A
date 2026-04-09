import React from 'react';

interface ConfirmModalProps {
  open: boolean;
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  onConfirm: () => void;
  onCancel: () => void;
}

export const ConfirmModal: React.FC<ConfirmModalProps> = ({
  open,
  title = 'Confirmar',
  message,
  confirmText = 'Eliminar',
  cancelText = 'Cancelar',
  onConfirm,
  onCancel,
}) => {
  if (!open) return null;
  return (
    <div className="modal-backdrop">
      <div className="modal">
        <div className="modal__header">
          <h3>{title}</h3>
        </div>
        <div className="modal__body">
          <p>{message}</p>
        </div>
        <div className="modal__actions">
          <button className="btn btn-danger" onClick={onConfirm}>{confirmText}</button>
          <button className="btn btn-secondary" onClick={onCancel}>{cancelText}</button>
        </div>
      </div>
      <style>{`
        .modal-backdrop {
          position: fixed;
          top: 0; left: 0; right: 0; bottom: 0;
          background: rgba(0,0,0,0.18);
          display: flex;
          align-items: center;
          justify-content: center;
          z-index: 1000;
        }
        .modal {
          background: #fff;
          border-radius: 12px;
          box-shadow: 0 2px 16px rgba(0,0,0,0.12);
          padding: 24px 28px;
          min-width: 320px;
          max-width: 90vw;
        }
        .modal__header h3 {
          margin: 0 0 12px 0;
        }
        .modal__body p {
          margin: 0 0 18px 0;
        }
        .modal__actions {
          display: flex;
          gap: 12px;
          justify-content: flex-end;
        }
      `}</style>
    </div>
  );
};
